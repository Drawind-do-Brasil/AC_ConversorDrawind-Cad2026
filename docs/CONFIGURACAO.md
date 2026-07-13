# Configuracao e arquivos .txml

A configuracao do conversor vive no projeto `ConversorDrawind.Shared`. O objeto central e `Configuration`, usado tanto pelo WPF quanto pelos comandos carregados no AutoCAD.

## Modelo principal

`Configuration` e composto por secoes:

| Secao | Classe | Conteudo |
| --- | --- | --- |
| Comentarios | `Comments` | Texto livre da configuracao. |
| Geral | `GeneralConfiguration` | Tipo de conversor, flags de conversao, formato, escala, Lisp/DLL, purge e explosao. |
| Cotas | `DimensionConfiguration` | Layer, estilo, cores, escala, precisao, setas e comportamento de texto de cotas. |
| Texto | `TextConfiguration` | Estilo padrao e lista de estilos. |
| Escala | `ScaleConfiguration` | Pontos de referencia, layer e tamanho de texto para escala. |
| Layers | `LayerConfiguration` | Layers base, novas layers, regras de remocao/conversao e layers para explosao. |
| Linhas | `LineConfiguration` | Escala de linetype e linetypes base. |
| Comandos | `CommandConfiguration` | Comandos Lisp e DLL configurados pelo usuario. |
| Blocos | `BlockConfiguration` | Blocos Tekla/CAD/originais, tags e bloco de dimensao. |
| Runtime | `RuntimeConfiguration` | Caminho do banco `.lin` e pasta temporaria. |
| Catalogos | `CatalogConfiguration` | Listas auxiliares usadas pela UI: cores, tipos de objeto, linetypes etc. |

## Formato .txml atual

O formato atual grava a raiz:

```xml
<CONVERSOR VERSION="2">
  ...
</CONVERSOR>
```

As secoes gravadas sao:

- `COMMENTS`
- `GENERAL`
- `DIMENSIONS`
- `TEXT`
- `SCALE`
- `LAYERS`
- `LINES`
- `CATALOGS`
- `COMMANDS`
- `BLOCKS`

A escrita fica em `StructuredConfigurationXmlWriter`. A leitura fica em `StructuredConfigurationXmlReader`.

## Compatibilidade com formato legado

Ao carregar um `.txml`, `ConverterConfigurationReader.Load` verifica o atributo `VERSION`.

- Se `VERSION="2"`, usa o leitor estruturado atual.
- Se nao houver `VERSION="2"`, usa `LegacyConfigurationXmlReader`.

O leitor legado aplica o XML antigo em `LegacyConfigurationState` e depois usa `ConfigurationCompatibilityMapper` para preencher o modelo novo.

Essa compatibilidade permite abrir configuracoes antigas sem manter o resto da aplicacao preso ao contrato antigo.

## Caminhos de configuracao

O servico `ConverterFileService` e a fachada de persistencia usada pelo WPF e pelos comandos.

Operacoes:

- `ListConverterNames(statusConversorItem)`: lista arquivos `.txml`.
- `GetTxmlPath(converterName, statusConversorItem)`: resolve caminho final.
- `LoadConverter(converterName, statusConversorItem)`: carrega uma configuracao.
- `SaveConverter(converterName, statusConversorItem, configuration)`: salva uma configuracao.

`ConfigurationPaths` delega para `ConfigurationXmlContract`, que centraliza a resolucao de pasta e nome do `.txml`.

## Escrita atomica

Arquivos importantes sao gravados por `AtomicFile`. A ideia e escrever primeiro em arquivo temporario no mesmo diretorio e publicar o resultado apenas quando a escrita terminou.

Isso vale para:

- `.txml` de conversores.
- `LinPack.nfj`.
- `LastConverter.nfj`.

## Preferencias por usuario

`UserSettingsService` salva preferencias em:

```text
%LocalAppData%\ConversorDrawind
```

Arquivos:

- `LinPack.nfj`: caminho do arquivo `.lin` configurado.
- `LastConverter.nfj`: ultimo status/pasta e conversor selecionado.

Se existir arquivo legado na pasta do executavel, o servico tenta migrar para `%LocalAppData%`.

## Defaults

`Configuration.EnsureDefaults()` garante que secoes e listas existam.

Alguns defaults importantes:

- Pasta temporaria: `%TEMP%\ConversorDrawindTemp\`.
- Caminho default do `.lin`: pasta de suporte do AutoCAD 2026.
- Estilo de texto default: gerado por `Defaults.TextStyle()`.
- Catalogos de cores, objetos e linetypes: gerados por `Defaults`.
- Novas layers iniciais: `Defaults.DefaultNewLayers()`.

Ao adicionar novo campo de configuracao:

1. Adicione a propriedade no modelo correto em `ConverterConfigurationModels.cs`.
2. Defina default coerente no construtor/propriedade ou em `EnsureDefaults`.
3. Grave o atributo em `StructuredConfigurationXmlWriter`.
4. Leia o atributo em `StructuredConfigurationXmlReader`.
5. Atualize mapeamento legado se o campo existir no formato antigo.
6. Atualize a UI WPF se o usuario precisa editar o campo.
7. Adicione teste de round-trip ou caracterizacao quando houver risco de regressao.
