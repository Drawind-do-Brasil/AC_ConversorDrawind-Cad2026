# Desenvolvimento, build e testes

## Ambiente recomendado

- Windows.
- .NET 8 SDK.
- Visual Studio com carga de trabalho ".NET desktop development".
- AutoCAD 2026 instalado e registrado no Windows.
- PowerShell.

## Solucao principal

Arquivo:

```text
ConversorDrawind.Cad2026.sln
```

Projetos incluidos:

- `ConversorDrawind`
- `ConversorDrawind.Shared`
- `ConversorDrawind.Commands`
- `ConversorDrawind.Tests`
- `ConversorDrawind.Commands.Tests`

## Build

Build da solucao:

```powershell
dotnet build ConversorDrawind.Cad2026.sln -c Debug -p:Platform=x64
```

Build do projeto de comandos com caminho explicito do AutoCAD:

```powershell
dotnet build ConversorDrawind.Commands\ConversorDrawind.Commands\ConversorDrawind.Commands.csproj -c Debug -p:AutoCADInstallDir="C:\Program Files\Autodesk\AutoCAD 2026\"
```

O projeto WPF referencia as DLLs Interop do AutoCAD em:

```text
C:\Program Files\Autodesk\AutoCAD 2026\
```

O projeto `Commands` aceita override por MSBuild:

```text
-p:AutoCADInstallDir="C:\Path\To\AutoCAD\"
```

## Testes

Rodar todos os testes da solucao:

```powershell
dotnet test ConversorDrawind.Cad2026.sln -c Debug -p:Platform=x64
```

Rodar testes WPF:

```powershell
dotnet test ConversorDrawind\ConversorDrawind.Tests\ConversorDrawind.Tests.csproj -c Debug -p:Platform=x64
```

Rodar testes Commands:

```powershell
dotnet test ConversorDrawind.Commands\ConversorDrawind.Commands.Tests\ConversorDrawind.Commands.Tests.csproj -c Debug -p:Platform=x64 -p:AutoCADInstallDir="C:\0_Programas\AutocadRefs\"
```

Observacoes:

- `ConversorDrawind.Commands.Tests` copia `accoremgd.dll`, `acdbmgd.dll` e `acmgd.dll` para a pasta de saida depois do build.
- O default de testes Commands e `C:\0_Programas\AutocadRefs\`.
- Se as referencias estiverem no AutoCAD instalado, passe `AutoCADInstallDir`.
- Podem aparecer avisos de conflito `WindowsBase` por dependencias do AutoCAD.
- Podem aparecer avisos `NU1900` se a origem NuGet estiver indisponivel.

## Testes de integracao AutoCAD

Ha suporte de integracao em:

```text
ConversorDrawind.Commands\ConversorDrawind.Commands.Tests\Integration
```

Arquivos importantes:

- `README.md`: instrucoes especificas da integracao.
- `Run-AutoCADIntegration.ps1`: script para executar cenarios com AutoCAD.
- `AutoCADIntegrationScriptBuilder.cs`: geracao de script AutoCAD.
- `Runs/*`: cenarios e manifests usados nos testes de smoke/integracao.

Esses testes dependem de AutoCAD real e devem ser tratados separadamente dos testes unitarios.

## Padroes de alteracao

### Novo campo no `.txml`

1. Atualize o modelo em `ConversorDrawind.Shared/Configuration/ConverterConfigurationModels.cs`.
2. Atualize escrita em `StructuredConfigurationXmlWriter`.
3. Atualize leitura em `StructuredConfigurationXmlReader`.
4. Atualize defaults em `Configuration.EnsureDefaults` se necessario.
5. Atualize compatibilidade legada se o campo existia no formato antigo.
6. Atualize a UI WPF.
7. Adicione teste de round-trip.

### Nova etapa funcional da conversao

1. Crie ou atualize servico em `ConversorDrawind.Commands/ConversorDrawind.Commands/Services`.
2. Chame a etapa em `ConversionWorkflow` ou no comando especifico.
3. Use `ConversionStepRunner` para log, warning e isolamento de falha.
4. Evite depender diretamente de `Configuration.Config`; receba `Configuration` quando possivel.
5. Extraia funcoes puras para permitir teste sem AutoCAD.

### Novo comando AutoCAD

1. Adicione arquivo em `ConversorDrawind.Commands/ConversorDrawind.Commands/Commands`.
2. Use `[CommandMethod("NOME_DO_COMANDO")]`.
3. Mantenha o metodo de entrada pequeno.
4. Delegue a logica para workflow/servico.
5. Documente no mapa de comandos em `docs/EXECUCAO.md`.

### Nova tela ou dialog WPF

1. Coloque XAML em `ConversorDrawind/ConversorDrawind/UI/Wpf`.
2. Se precisar compatibilidade com chamadas legadas, crie adaptador em `UI/Adapters`.
3. Centralize estado de edicao em view model quando possivel.
4. Persistencia deve continuar passando por `ConverterFileService`.

## Pontos de atencao

- Os projetos de producao estao com `Nullable` desabilitado. Ao alterar codigo sensivel, trate nulos defensivamente.
- Ainda ha partes com nomes legados em portugues/ingles misturados. Evite renomeacoes amplas sem necessidade.
- Evite `catch (Exception)` vazio em codigo novo.
- Evite adicionar mais estado estatico mutavel.
- Qualquer chamada COM ao AutoCAD pode precisar de `ComRetry` e `MessageFilter.ScopedRegistration`.
- Nunca escreva preferencias na pasta do executavel; use `UserSettingsService`.

## Checklist antes de concluir uma mudanca

- Build da solucao ou do projeto afetado.
- Testes do projeto afetado.
- Round-trip de `.txml` quando a mudanca envolver configuracao.
- Validacao manual no AutoCAD quando a mudanca depender da API real.
- Atualizacao desta documentacao quando houver novo fluxo, comando ou contrato.
