# Roteiro de integracao AutoCAD

Este diretorio guarda o roteiro da Fase 1/Fase 9 para validar a DLL em um DWG real.

## Execucao automatizavel

Quando houver um DWG de referencia e um `.txml` conhecido, rode:

```powershell
.\Run-AutoCADIntegration.ps1 `
  -DwgPath "C:\caminho\referencia.dwg" `
  -TxmlPath "C:\caminho\configuracao.txml"
```

O script:

- copia o DWG para uma pasta de saida;
- escreve `%TEMP%\ConversorDrawind.Temp` apontando para o `.txml`;
- gera um `run-conversor.scr`;
- define `SECURELOAD` como `0` durante o roteiro para permitir `NETLOAD` no Core Console;
- carrega `ConversorDrawind.dll` via `NETLOAD`;
- executa `DRAWINDCAD_Convert`;
- salva a copia do DWG com `QSAVE`;
- grava o log do `accoreconsole`.
- grava `integration-manifest.json` com caminhos, hashes SHA-256, comandos, horario, status e codigo de saida.

Mesmo quando o `accoreconsole.exe` retorna codigo `0`, o script marca a execucao como falha se o log indicar que a DLL nao foi carregada, se algum comando esperado aparecer como desconhecido ou se houver excecao/fatal error.

Parametros opcionais:

- `-DllPath`: caminho da DLL compilada.
- `-CoreConsolePath`: caminho do `accoreconsole.exe`.
- `-Commands`: comandos AutoCAD a executar, por padrao `DRAWINDCAD_Convert`.
- `-OutputDirectory`: pasta onde a copia do DWG, script e logs serao gravados.

## Checklist manual de aceite

Depois da execucao, comparar a copia convertida com o DWG esperado:

- layers criadas/removidas;
- estilos de texto;
- estilos de cotas;
- cotas, setas, textos e posicoes;
- blocos, atributos e escala;
- logs do conversor;
- ausencia de erros no log do `accoreconsole`.
- manifesto `integration-manifest.json` apontando para o DWG, `.txml`, DLL e comandos corretos.

## Observacao

O workspace atual ainda nao tem um DWG de referencia versionado. Quando um DWG de exemplo for adicionado, este arquivo deve receber o nome do desenho, o `.txml` usado e os comandos esperados.
