# Conversor Drawind CAD 2026

Conversor Drawind CAD 2026 e uma solucao .NET 8 para configurar e executar conversoes de desenhos AutoCAD. Depois da refatoracao, o projeto ficou dividido em tres partes principais:

- `ConversorDrawind.Shared`: modelos, defaults, caminhos, compatibilidade e persistencia de configuracao.
- `ConversorDrawind`: aplicativo WPF usado para configurar conversores e executar lotes de desenhos via AutoCAD COM.
- `ConversorDrawind.Commands`: biblioteca carregada dentro do AutoCAD, contendo os comandos `CDwi_*` e a logica funcional da conversao.

Tambem existem dois projetos de teste:

- `ConversorDrawind.Tests`: testes do aplicativo WPF e componentes compartilhados com a aplicacao.
- `ConversorDrawind.Commands.Tests`: testes de workflows, funcoes puras, caracterizacao de configuracao e integracao assistida com AutoCAD.

## Documentacao

- [Arquitetura](docs/ARQUITETURA.md)
- [Configuracao e arquivos .txml](docs/CONFIGURACAO.md)
- [Execucao da conversao](docs/EXECUCAO.md)
- [Desenvolvimento, build e testes](docs/DESENVOLVIMENTO.md)

## Requisitos principais

- Windows.
- .NET 8 SDK.
- Visual Studio com suporte a WPF/.NET Desktop.
- AutoCAD 2026 instalado para build e execucao dos projetos que referenciam as DLLs Autodesk.
- Referencias gerenciadas do AutoCAD:
  - `accoremgd.dll`
  - `acdbmgd.dll`
  - `acmgd.dll`
  - `Autodesk.AutoCAD.Interop.dll`
  - `Autodesk.AutoCAD.Interop.Common.dll`

## Build rapido

Na raiz da solucao:

```powershell
dotnet build ConversorDrawind.Cad2026.sln -c Debug -p:Platform=x64
```

Se as referencias gerenciadas do AutoCAD estiverem em outro diretorio para o projeto `Commands`, informe:

```powershell
dotnet build ConversorDrawind.Commands\ConversorDrawind.Commands\ConversorDrawind.Commands.csproj -c Debug -p:AutoCADInstallDir="C:\Program Files\Autodesk\AutoCAD 2026\"
```

## Fluxo resumido

1. O usuario configura um conversor no WPF.
2. O WPF salva/carrega a configuracao `.txml` usando `ConversorDrawind.Shared`.
3. Na execucao, o WPF abre o AutoCAD via COM.
4. O WPF carrega a DLL `ConversorDrawind.Commands` no AutoCAD.
5. Para cada desenho, o WPF envia comandos AutoCAD como `CDwi_Convert`, `CDwi_DeleteLayers`, `CDwi_Scale` e `CDwi_Finalize`.
6. Os comandos executam a conversao real dentro do processo do AutoCAD.
7. O lote registra arquivos convertidos, falhas e progresso.

## Onde comecar

Para entender a separacao pos-refatoracao, leia primeiro [docs/ARQUITETURA.md](docs/ARQUITETURA.md). Para mexer na estrutura do `.txml`, leia [docs/CONFIGURACAO.md](docs/CONFIGURACAO.md). Para alterar a conversao funcional, comece por [docs/EXECUCAO.md](docs/EXECUCAO.md).
