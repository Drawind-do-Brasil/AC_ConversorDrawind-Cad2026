# Revisao tecnica pos-refatoracao

Data da revisao: 10/07/2026  
Escopo: `ConversorDrawind.Shared`, `ConversorDrawind` (WPF) e `ConversorDrawind.Commands`.

## Resumo executivo

A separacao em tres projetos e a criacao de workflows, adaptadores e testes de caracterizacao representam uma evolucao positiva. Os ciclos desta revisao trataram os pontos de maior risco para que a aplicacao fique mais previsivel, testavel e facil de manter:

1. As duas suites foram recuperadas e passaram: 88 testes de Commands e 16 testes WPF.
2. O processamento por desenho agora informa sucesso/falha, evita salvar falhas e libera a sessao COM de forma deterministica.
3. A persistencia de configuracoes e preferencias usa escrita atomica e dados por usuario.
4. `DrawingProcess` foi dividido por responsabilidade, e os fluxos principais de Commands recebem a configuracao carregada de forma explicita.

> Atualizacao: os ciclos 1 e 2 foram implementados apos a revisao inicial.

## Execucao das melhorias

| Ciclo | Status | Entrega |
| --- | --- | --- |
| 1 - Base de testes | [x] Concluido | Corrigidos os `ProjectReference`/usings legados, incluidos os dois projetos de teste na solucao principal e atualizados os testes para as APIs refatoradas. |
| 2 - Persistencia segura | [x] Concluido | Preferencias migradas para `LocalApplicationData`, com migracao de arquivos legados e gravacao atomica. O salvamento de `.txml` tambem passou a usar gravacao atomica. |
| 3 - Resultado por desenho | [x] Concluido | Cada desenho agora devolve sucesso/falha; falhas nao disparam salvamento e nao entram na lista de arquivos convertidos. O fim do lote informa quantos desenhos falharam. |
| 4 - Sessao COM e orquestracao WPF | [x] Concluido | Eventos COM agora sao desassinados e referencias COM sao liberadas ao encerrar. `AutoCadSession` passou a concentrar o estado da sessao, a automacao Win32 foi isolada, comandos LISP foram extraidos e o lote foi separado em `DrawingProcess.Batch.cs`. |
| 5 - Limpeza tecnica | [x] Concluido | Removida a implementacao criptografica legada sem consumidores internos. As colecoes globais de blocos passaram a ser expostas somente para leitura. |
| 6 - Validacao final | [x] Concluido | Solucao compilada e suites executadas com sucesso: 88 testes de Commands e 16 testes WPF. Os avisos de `WindowsBase` foram registrados como ponto de compatibilidade das referencias do AutoCAD. |
| 7 - Contexto de configuracao em Commands | [x] Concluido | `ConversionWorkflow`, comandos de escala/atributos/finalizacao e os servicos de dimensao/escala agora capturam ou recebem a configuracao carregada como dependencia explicita. Os acessos globais restantes estao nas fronteiras de compatibilidade e em utilitarios legados. |
| 8 - Workflow por desenho | [x] Concluido | `RunCommand` foi reduzido a uma fachada e a sequencia de abrir, converter, copiar formato, executar LISP, salvar e fechar foi movida para `DrawingConversionWorkflow`. As propriedades estaticas de progresso foram simplificadas. |

## Validacao executada

| Verificacao | Resultado |
| --- | --- |
| Build WPF, Commands e Shared | Concluido com sucesso pelo MSBuild do Visual Studio, usando as referencias do AutoCAD 2026 instaladas. |
| `ConversorDrawind.Commands.Tests` | 88 testes aprovados. |
| `ConversorDrawind.Tests` | 16 testes aprovados. |
| Avisos remanescentes | Ha avisos `MSB3277` de conflito entre `WindowsBase` do .NET e dependencias do AutoCAD. Tambem pode ocorrer `NU1900` quando a origem do NuGet estiver indisponivel. Nenhum dos dois bloqueou a compilacao ou os testes desta revisao. |

## Achados priorizados

### P1 — Erros de conversao sao ocultados e o desenho pode ser salvo parcialmente — **corrigido no ciclo 3**

**Onde:** `ConversorDrawind/ConversorDrawind/Conversion/DrawingProcess.cs`, metodo `RunCommand`, linhas 505–510.

O bloco que executa a conversao captura qualquer `Exception`, apenas escreve a mensagem no `Debug` e continua para a rotina de salvamento. Isso pode produzir um DWG/DXF que aparenta ter sido convertido, embora parte das etapas tenha falhado.

**Impacto:** perda de confiabilidade e diagnostico dificil em ambiente do usuario, especialmente porque o `Debug.WriteLine` normalmente nao chega ao usuario final.

**Recomendacao:** fazer o resultado de cada arquivo ser explicito (`Success`, `Failed`, `Cancelled`), registrar excecao completa e impedir o salvamento quando uma etapa critica falhar. O log final deve indicar, por arquivo, quais etapas concluíram e qual foi a causa da falha.

### P1 — A suite de testes do projeto Commands esta quebrada — **corrigido no ciclo 1**

**Onde:** `ConversorDrawind.Commands/ConversorDrawind.Commands.Tests/ConversorDrawind.Commands.Tests.csproj`, linha 28.

O projeto referencia `..\\ConversorDrawindDLL\\ConversorDrawind.Commands.csproj`, mas o projeto real esta em `..\\ConversorDrawind.Commands\\ConversorDrawind.Commands.csproj`.

**Resultado da correcao:** o caminho foi atualizado, os testes foram adequados as APIs extraidas e a suite passou com 88 testes aprovados.

**Impacto:** os testes de caracterizacao e de funcoes puras nao protegem o codigo contra regressao.

**Pendencia residual:** eliminar os avisos `MSB3277` de `WindowsBase` na configuracao de referencias do AutoCAD.

### P1 — A solucao principal nao executa os testes — **corrigido no ciclo 1**

**Onde:** `ConversorDrawind.Cad2026.sln`.

Os projetos `ConversorDrawind.Tests` e `ConversorDrawind.Commands.Tests` existem no repositorio, mas nao estao incluidos na solucao. Assim, o comando normal de validacao da solucao nao cobre nenhum teste.

**Impacto:** uma validacao local ou de CI pode indicar sucesso sem testar comportamento algum.

**Resultado da correcao:** ambos os projetos foram incluidos na solucao principal e as duas suites estao executaveis.

### P2 — `DrawingProcess` concentrava responsabilidades demais e estado global — **corrigido no ciclo 4**

**Onde:** `ConversorDrawind/ConversorDrawind/Conversion/DrawingProcess.cs` (915 linhas).

A classe combina automacao COM do AutoCAD, interoperabilidade Win32, monitoramento de comandos, abertura/fechamento de documentos, processo em lote, regras de conversao, comandos LISP, tratamento de arquivos temporarios, logging e progresso de interface. Tambem usa estado estatico compartilhado (`myClass`, `parametros`, documento atual, flags e progresso).

**Impacto:** o comportamento fica dificil de testar sem AutoCAD, fragil em reentrancia/concorrencia e custoso de alterar. Uma falha em uma responsabilidade pode afetar todas as outras.

**Recomendacao de divisao:**

| Componente sugerido | Responsabilidade |
| --- | --- |
| `AutoCadComSession` | Criar, conectar, encerrar e liberar a sessao COM; assinar e desassinar eventos. |
| `AutoCadCommandExecutor` | Enviar comando, aguardar conclusao e aplicar timeout/retry. |
| `DrawingConversionOrchestrator` | Executar as etapas de conversao de um unico desenho e devolver resultado estruturado. |
| `BatchConversionService` | Percorrer a lista, cancelar, calcular progresso e consolidar log. |
| `NativeWindowService` | Isolar chamadas Win32 usadas para localizar a janela e enviar teclas. |

**Resultado da correcao:** o estado de sessao foi concentrado em `AutoCadSession`, a automacao de janela foi isolada em `AutoCadWindowActivator`, a interpretacao de comandos LISP ganhou tipo proprio e o processamento em lote foi movido para `DrawingProcess.Batch.cs`. A classe principal deixou de concentrar essas responsabilidades diretamente.

### P2 — Eventos e referencias COM nao sao liberados de forma deterministica — **corrigido no ciclo 4**

**Onde:** `DrawingProcess.cs`, linhas 175–176 e metodo `CloseACAD`.

Os eventos `BeginCommand` e `EndCommand` sao assinados ao abrir o AutoCAD, mas nao sao removidos no encerramento. Tambem nao ha uma rotina central de limpeza das referencias COM da aplicacao e dos documentos.

**Impacto:** em conversoes repetidas pode haver objetos COM pendurados, handlers retendo referencias e processos do AutoCAD que nao terminam quando esperado.

**Recomendacao:** encapsular a sessao em um objeto descartavel; em `Dispose`, desassinar os eventos, fechar somente os documentos que foram abertos pelo conversor, liberar referencias COM de modo controlado e zerar o estado da sessao. Evitar `GC.Collect` como mecanismo primario de limpeza.

### P2 — Preferencias do usuario sao salvas na pasta de instalacao e sem escrita atomica — **corrigido no ciclo 2**

**Onde:** `ConversorDrawind.Shared/Configuration/UserSettingsService.cs`, linhas 11–46.

`LinPack.nfj` e `LastConverter.nfj` sao gravados em `AppDomain.CurrentDomain.BaseDirectory`. Em uma instalacao convencional, a pasta do executavel pode estar protegida contra escrita. Alem disso, `SaveProgramDbLin` exclui o arquivo existente antes de criar o novo, deixando uma janela de perda de dados.

**Impacto:** configuracao pode deixar de ser salva em maquinas de usuarios ou ser perdida em caso de queda/interrupcao durante a escrita.

**Recomendacao:** usar `%LocalAppData%\\<Empresa>\\<Produto>` para dados por usuario. Gravar primeiro em arquivo temporario no mesmo diretorio e substituir o arquivo final somente apos o fechamento bem-sucedido do stream. Usar `using` em todas as operacoes de leitura/escrita.

### P2 — Salvamento de `.txml` pode corromper a configuracao existente — **corrigido no ciclo 2**

**Onde:** `ConversorDrawind.Shared/Configuration/ConverterConfigurationXml.cs`, linhas 85–102.

O XML e escrito diretamente no caminho final. Se houver queda do processo, falta de espaco ou interrupcao no meio da serializacao, o arquivo existente pode ficar parcial ou invalido.

**Impacto:** o usuario pode perder a configuracao do conversor e so perceber ao carregar ou executar uma conversao.

**Recomendacao:** salvar em `arquivo.txml.tmp`, validar que a escrita terminou e usar `File.Replace` (quando o arquivo ja existir) ou `File.Move` para publicar o resultado. Manter backup opcional da ultima configuracao valida.

### P3 — Estado de configuracao em tempo de execucao ainda era global e mutavel — **corrigido nos ciclos 5 e 7**

**Onde:** `ConversorDrawind.Commands/ConversorDrawind.Commands/Workflows/RuntimeConfigurationState.cs`, linhas 9–11 e 59–63.

As listas de blocos sao estaticas e expostas como `List<Block>`, permitindo que qualquer comando as altere. O mesmo vale para o uso recorrente de `Configuration.Config` como fonte global.

**Impacto:** efeitos colaterais entre comandos, dependencia de ordem de execucao e dificuldade de executar cenarios isolados em testes.

**Resultado da correcao:** as colecoes de blocos ficaram encapsuladas e expostas somente para leitura. Os fluxos de conversao, escala, atributos, finalizacao e dimensoes passaram a receber ou capturar a configuracao carregada como dependencia explicita. Os acessos globais que restam ficam restritos a carregamento, compatibilidade e utilitarios legados, o que permite sua substituicao gradual sem alterar a fronteira publica dos comandos.

### P3 — Ha uma implementacao de criptografia legada possivelmente sem uso — **corrigido no ciclo 5**

**Onde:** `ConversorDrawind/ConversorDrawind/Security/Crypt.cs`.

A classe oferece RC2, DES e TripleDES, usa IV fixo e APIs antigas como `RijndaelManaged` e `PasswordDeriveBytes`. Nao foi localizado uso de `Crypt`, `Encrypt` ou `Decrypt` nos arquivos C# do repositorio.

**Impacto:** codigo morto aumenta manutencao; se voltar a ser usado para dados sensiveis, a implementacao nao oferece protecao adequada.

**Recomendacao:** remover se nao houver consumidor externo. Se houver compatibilidade historica obrigatoria, isolar como leitor legado e implementar novas gravacoes com AES autenticado, salt/nonce aleatorios e derivacao moderna de chave.

## Melhorias adicionais recomendadas

- Habilitar `Nullable` nos projetos de producao gradualmente. Hoje ele esta desabilitado em Shared, WPF e Commands, ocultando possiveis `NullReferenceException`.
- Remover `catch (Exception)` vazios ou que apenas relancam a mesma excecao; cada captura deve ter uma acao definida, contexto de log ou uma justificativa clara de recuperacao.
- Padronizar nomes e acessibilidade: ainda coexistem modelos e nomes de legado como `PointEspecial`, `TagBlock`, campos publicos em minusculo e nomes em portugues/ingles. Migrar de forma incremental, mantendo adaptadores apenas nas fronteiras de compatibilidade.
- Centralizar os caminhos de referencias do AutoCAD em um arquivo de propriedades local/CI, evitando valores diferentes entre o projeto Commands e seus testes.

## Ordem sugerida de execucao

1. Corrigir e incluir os projetos de teste na solucao; fazer a suite compilar e rodar no CI.
2. Impedir que uma conversao com falha seja salva como sucesso e melhorar o log por arquivo.
3. Extrair a sessao COM e o executor de comandos de `DrawingProcess`; cobrir o orquestrador com testes unitarios.
4. Tornar escrita de configuracao e preferencias atomica e movida para o perfil do usuario.
5. Substituir estado global de runtime por contexto por execucao e remover codigo criptografico legado nao utilizado.
