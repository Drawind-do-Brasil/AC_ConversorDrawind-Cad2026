# Roteiro de modernizacao da configuracao do ConversorDrawind

## Objetivo

Modernizar `Configuration.cs` para que ela deixe de ser apenas um saco de campos publicos e passe a representar, carregar e salvar uma configuracao completa do conversor.

A nova estrutura deve:

- Usar nomes claros e agrupados por responsabilidade.
- Encapsular os dados que hoje ficam soltos fora da configuracao: `Arranjos`, `List<Block> blocks`, `List<Block> blocosi`, `List<Block> blocoso` e o contexto de arquivo baseado em `StatusConversorItem`.
- Ler os arquivos `.txml` legados existentes em `TemplatesAtivos`.
- Salvar em um formato estruturado novo, sem depender de strings compostas com separadores como `:`, `;` e `@`.
- Preservar uma camada de compatibilidade para que a UI e a DLL possam ser migradas aos poucos.

## Situacao atual

Arquivo principal:

`ConversorDrawind.Shared/Configuration/Configuration.cs`

Problemas principais:

- Campos publicos com prefixos historicos (`EXTDIM`, `EXTCONF`, `PROGRAM`, `DM`) misturam persistencia, UI, runtime e regras de conversao.
- `Configuration` depende de parametros externos para salvar e carregar: `Arranjos`, `blocks`, `blocosi`, `blocoso` e `StatusConversorItem`.
- Parte do estado real da configuracao nao mora dentro de `Configuration`.
- O contrato XML atual (`ConfigurationXmlDocument`) ja agrupa secoes, mas ainda serializa varios conceitos como strings compostas:
  - `NEW_LAYER`: `Nome:Cor:TipoLinha`
  - `TEXT_STYLE`: `Nome:Fonte:Italico:Negrito:Tamanho:FatorLargura:AnguloObliquo`
  - `REMOVE_LAYER`: `LayerBase;Filtro`
  - `CONVERTER`: `LayerBase;FiltroOrigem;LayerDestino:Cor:TipoLinha:::EstiloTexto`
  - `TAG`: `Tag@Modify@P1;P2@LayerBase;Filtro@WidthFactor`
  - `BLOCK_ATT_CAD` e `BLOCK_ATT_ORIG`: `Nome;NomeRelacionado;CorArgb`
- `ConfigurationXmlContract.cs` duplica parte dos nomes do contrato e deve ser revisado ou removido quando o novo contrato estiver consolidado.

## Modelo proposto

Criar uma configuracao raiz com estado completo:

```csharp
public sealed class ConverterConfiguration
{
    public string Comments { get; set; }
    public GeneralConfiguration General { get; set; }
    public DimensionConfiguration Dimensions { get; set; }
    public TextConfiguration Text { get; set; }
    public ScaleConfiguration Scale { get; set; }
    public LayerConfiguration Layers { get; set; }
    public LineConfiguration Lines { get; set; }
    public CommandConfiguration Commands { get; set; }
    public BlockConfiguration Blocks { get; set; }
    public RuntimeConfiguration Runtime { get; set; }
}
```

Classes sugeridas:

- `GeneralConfiguration`
  - `SourceMode` no lugar de `EXTCONFOrigem`.
  - `ConvertDimensions` no lugar de `EXTCONFIsConvertDimension`.
  - `ConvertLayers` no lugar de `EXTCONFIsConvertLayer`.
  - `ExchangeFormat` no lugar de `EXTCONFIsExchangeFormat`.
  - `ApplyDrawingScale` no lugar de `EXTCONFIsPutOnTheScaleDrawing`.
  - `ExecuteLisp` ou `AutomationMode` no lugar de `EXTCONFIsExecuteLISP` e `EXTCONFIsFirstRum`.
  - `ExecuteDll` no lugar de `EXTCONFIsExecuteDLL`.
  - `DeleteTeklaStructures` no lugar de `EXTCONFIsDeleteTeklaStructures`.
  - `Purge` no lugar de `EXTCONFIsPurge`.
  - `ShowMessages` no lugar de `PROGRAMMessage`.
  - `ExplodeBlocks` no lugar de `ExplodeBlocks`.
  - `InventorExplode` no lugar de `EXTCONFInventorExplode`.
  - `ConverterType` no lugar de `ConvTekla0ConvInv1`.

- `DimensionConfiguration`
  - `Enabled` no lugar de `EXTDIMGERALHabilit`.
  - `Layer`, `BaseLayer`, `StyleName`, `Scale`, `Precision`, `AngularPrecision`.
  - `Unit`, `AngularUnit`.
  - `ArrowType`, `ArrowType1`, `ArrowType2`, `ArrowSize`.
  - `TextVerticalPosition` no lugar de `EXTDIMTad`.
  - `TextRelativeToDimensionLine` no lugar de `EXTDIMDimensionPosition`.
  - `ForceTextInside`, `ForceDimensionLine`.
  - `TextColor`, `LineColor`.
  - `OffsetLineFromReferencePoint`, `TextMove`, `OutsideAlign`.
  - `ExtensionLineOffset` no lugar de `EXTDIMDIMEX`.
  - `FixArrow`, `FixArrowType`, `FixArrowFactor`.

- `TextConfiguration`
  - `DefaultStyleName` no lugar de `EXTTEXTStyleName`.
  - `DefaultSize` no lugar de `EXTTEXTSize`.
  - `Styles` como `List<TextStyleDefinition>` no lugar de `Arranjos.allTextSyles`.

- `ScaleConfiguration`
  - `Manual` no lugar de `EXTSCALEManual`.
  - `Point1`, `Point2` como objeto `Point3DConfiguration`.
  - `Layer`, `TextSize`.

- `LayerConfiguration`
  - `TeklaDrawingSheetLayer` no lugar de `LayerTeklaString`.
  - `BlockAttributeLayer` no lugar de `LayerBlockAttribute`.
  - `BaseLayers` no lugar de `Arranjos.allBaseLayer`.
  - `NewLayers` como `List<LayerDefinition>` no lugar de `Arranjos.allNewLayerComposition`.
  - `RemoveRules` como `List<LayerRemoveRule>` no lugar de `Arranjos.layerRemove`.
  - `ConversionRules` como `List<LayerConversionRule>` no lugar de `Arranjos.conversor`.
  - `ExplodeLayers` no lugar de `Arranjos.allExplodeLayers`.

- `LineConfiguration`
  - `LineTypeScale` no lugar de `EXTLINELtscale`.
  - `BaseLineTypes` no lugar de `Arranjos.allLineType1`.

- `CommandConfiguration`
  - `LispCommands` no lugar de `Arranjos.listLISPCommand`.
  - `DllCommands` no lugar de `Arranjos.listDLLCommand`.

- `BlockConfiguration`
  - `TeklaBlockPath` no lugar de `PROGRAMblockFormatoCaminho`.
  - `CadBlockPath` no lugar de `EXTCONFCaminhoBlocoInv`.
  - `TeklaBlocks` no lugar de `blocks`.
  - `CadBlocks` no lugar de `blocosi`.
  - `OriginalBlocks` no lugar de `blocoso`.
  - `DimensionBlockEnabled` no lugar de `DMBlock`.

- `RuntimeConfiguration`
  - `DbLineTypePath` no lugar de `PROGRAMDbLin`.
  - `TempDirectory` no lugar de `PROGRAMDirectoryTemp`.
  - Manter fora do XML principal quando for dado local de maquina e nao parte do template.

## Objetos tipados para substituir strings compostas

```csharp
public sealed class LayerDefinition
{
    public string Name { get; set; }
    public string Color { get; set; }
    public string LineType { get; set; }
}

public sealed class TextStyleDefinition
{
    public string Name { get; set; }
    public string Font { get; set; }
    public bool Italic { get; set; }
    public bool Bold { get; set; }
    public double Size { get; set; }
    public double WidthFactor { get; set; }
    public double ObliqueAngle { get; set; }
}

public sealed class EntityFilter
{
    public string BaseLayer { get; set; }
    public string ObjectType { get; set; }
    public string Color { get; set; }
    public string LineType { get; set; }
    public string TextContent { get; set; }
    public string TextHeight { get; set; }
    public string Orientation { get; set; }
}

public sealed class LayerRemoveRule
{
    public EntityFilter Filter { get; set; }
}

public sealed class LayerConversionRule
{
    public EntityFilter Source { get; set; }
    public LayerOutput Target { get; set; }
}

public sealed class LayerOutput
{
    public string LayerName { get; set; }
    public string Color { get; set; }
    public string LineType { get; set; }
    public string TextStyle { get; set; }
}

public sealed class BlockDefinition
{
    public string Name { get; set; }
    public string RelatedName { get; set; }
    public int ColorArgb { get; set; }
    public List<BlockTagDefinition> Tags { get; set; }
}

public sealed class BlockTagDefinition
{
    public string Name { get; set; }
    public bool Modify { get; set; }
    public Point3DConfiguration Point1 { get; set; }
    public Point3DConfiguration Point2 { get; set; }
    public EntityFilter Filter { get; set; }
    public double WidthFactor { get; set; }
    public int RelatedIndex { get; set; }
    public bool IsAssociated { get; set; }
}
```

## Novo contrato de arquivo

Manter extensao `.txml`, mas gravar com versao explicita:

```xml
<CONVERSOR VERSION="2">
  <COMMENTS TEXT="" />
  <GENERAL SOURCE_MODE="Tekla" CONVERT_DIMENSIONS="true" CONVERT_LAYERS="true" PURGE="true" />
  <DIMENSIONS ENABLED="true" LAYER="0" STYLE_NAME="COTAS" />
  <TEXT DEFAULT_STYLE_NAME="TEXTO">
    <STYLE NAME="TEXTO" FONT="RomanS" ITALIC="false" BOLD="false" SIZE="2.5" WIDTH_FACTOR="1" OBLIQUE_ANGLE="0" />
  </TEXT>
  <LAYERS>
    <BASE_LAYER NAME="DRAWING SHEET" />
    <NEW_LAYER NAME="DWI01" COLOR="RED" LINE_TYPE="CONTINUOUS" />
    <REMOVE_RULE>
      <FILTER BASE_LAYER="REMOVER_WHITE" OBJECT_TYPE="ALL" COLOR="ALL" LINE_TYPE="ALL" TEXT_CONTENT="" TEXT_HEIGHT="" ORIENTATION="ALL" />
    </REMOVE_RULE>
    <CONVERSION_RULE>
      <SOURCE BASE_LAYER="ALL" OBJECT_TYPE="TEXT" COLOR="ALL" LINE_TYPE="ALL" TEXT_CONTENT="" TEXT_HEIGHT="" ORIENTATION="ALL" />
      <TARGET LAYER_NAME="DWI02TX" COLOR="BYLAYER" LINE_TYPE="BYLAYER" TEXT_STYLE="TEXTO" />
    </CONVERSION_RULE>
  </LAYERS>
  <BLOCKS TEKLA_BLOCK_PATH=" " CAD_BLOCK_PATH="" DIMENSION_BLOCK_ENABLED="false">
    <TEKLA_BLOCK NAME="FORMATO">
      <TAG NAME="TAG1" MODIFY="true" WIDTH_FACTOR="1">
        <POINT1 X="0" Y="0" Z="0" />
        <POINT2 X="0" Y="0" Z="0" />
        <FILTER BASE_LAYER="ALL" OBJECT_TYPE="TEXT" COLOR="ALL" LINE_TYPE="ALL" TEXT_CONTENT="" TEXT_HEIGHT="" ORIENTATION="ALL" />
      </TAG>
    </TEKLA_BLOCK>
    <CAD_BLOCK NAME="A1" RELATED_NAME="FORMATO_A1" COLOR_ARGB="-16777216" />
    <ORIGINAL_BLOCK NAME="A1_ORIG" RELATED_NAME="FORMATO_A1" COLOR_ARGB="-16777216" />
  </BLOCKS>
</CONVERSOR>
```

## Compatibilidade com arquivos legados

Criar uma fronteira explicita:

- `ConverterConfigurationReader`
  - Detecta `VERSION`.
  - Se `VERSION` ausente, usa leitor legado.
  - Se `VERSION="2"`, usa leitor estruturado.

- `LegacyConfigurationXmlReader`
  - Reaproveita `ConfigurationXmlDocument.Load(file)`.
  - Converte o documento legado para `ConverterConfiguration`.
  - Centraliza parsers de `:`, `;` e `@`.

- `StructuredConfigurationXmlWriter`
  - Salva sempre no formato `VERSION="2"`.
  - Nao emite strings compostas.

- `StructuredConfigurationXmlReader`
  - Le formato novo.
  - Faz fallback de defaults quando campos opcionais nao existirem.

Regra importante: leitura deve aceitar legado; escrita deve produzir somente estruturado.

## Camada de adaptadores

Para evitar uma mudanca enorme na UI e no runtime de uma vez, criar adaptadores temporarios:

```csharp
public static class ConfigurationCompatibilityMapper
{
    public static ConverterConfiguration FromLegacyState(
        Configuration configuration,
        Arranjos arranjos,
        List<Block> teklaBlocks,
        List<Block> cadBlocks,
        List<Block> originalBlocks);

    public static void ApplyToLegacyState(
        ConverterConfiguration source,
        Configuration configuration,
        Arranjos arranjos,
        List<Block> teklaBlocks,
        List<Block> cadBlocks,
        List<Block> originalBlocks);
}
```

Isso permite:

- `MainWindow.xaml.cs` continuar usando `configuration`, `arranjos`, `listBlocks`, `listBlocksInv`, `listBlocksOrig` inicialmente.
- `ConverterFileService` migrar primeiro para `ConverterConfiguration`.
- A DLL continuar recebendo estado legado enquanto os pontos de chamada sao modernizados aos poucos.

## Nova API de salvamento e carregamento

Criar um servico responsavel por arquivo:

```csharp
public interface IConverterConfigurationRepository
{
    bool Exists(string converterName, StatusConversorItem status);
    ConverterConfiguration Load(string converterName, StatusConversorItem status);
    void Save(string converterName, StatusConversorItem status, ConverterConfiguration configuration);
}
```

Implementacao inicial:

```csharp
public sealed class TxmlConverterConfigurationRepository : IConverterConfigurationRepository
{
    public bool Exists(string converterName, StatusConversorItem status);
    public ConverterConfiguration Load(string converterName, StatusConversorItem status);
    public void Save(string converterName, StatusConversorItem status, ConverterConfiguration configuration);
}
```

`StatusConversorItem` deve continuar sendo contexto de caminho, nao dado interno do template. A configuracao pode conhecer o repositorio, mas o template salvo nao precisa salvar o status.

## Etapas de implementacao

1. Criar modelos novos em `ConversorDrawind.Shared/Configuration/Models` ou `ConversorDrawind.Shared/Models/Configuration`.
2. Criar parsers pequenos e testaveis para os formatos legados:
   - `LegacyLayerDefinitionParser`
   - `LegacyTextStyleParser`
   - `LegacyEntityFilterParser`
   - `LegacyLayerConversionRuleParser`
   - `LegacyBlockParser`
   - `LegacyBlockTagParser`
3. Criar `ConverterConfigurationReader` com deteccao de versao.
4. Criar `StructuredConfigurationXmlWriter` e `StructuredConfigurationXmlReader`.
5. Criar `ConfigurationCompatibilityMapper`.
6. Alterar `ConverterFileService` para usar `ConverterConfiguration` internamente e adaptar para a API antiga.
7. Manter os metodos antigos de `Configuration` como obsoletos por uma fase:
   - `LoadXML(...)`
   - `SaveXML(...)`
   - `Load(...)`
   - `CheckFileTxmlExist(...)`
8. Depois que UI e DLL forem migradas, remover parametros soltos dos metodos publicos.
9. Transformar `Configuration` antiga em fachada ou substitui-la por `ConverterConfiguration`.

## Testes recomendados

Criar testes unitarios em `ConversorDrawind.Commands/ConversorDrawindDLL.Tests` ou em um projeto de testes compartilhado:

- Ler cada `.txml` legado em `TemplatesAtivos` sem excecao.
- Ler legado e salvar como `VERSION="2"`.
- Ler novamente o arquivo salvo em `VERSION="2"`.
- Comparar contagens:
  - camadas base
  - novas camadas
  - regras de remocao
  - regras de conversao
  - comandos
  - blocos Tekla
  - blocos CAD
  - blocos originais
  - tags por bloco
- Testar parsers isoladamente com exemplos reais:
  - `0:WHITE:CONTINUOUS`
  - `TEXTO:RomanS:false:false:2.5:1:0`
  - `REMOVER_WHITE;ALL:ALL:ALL:::ALL`
  - `ALL;TEXT:ALL:ALL:::ALL;DWI02TX:BYLAYER:BYLAYER:::TEXTO`
  - tags com `@`.

## Riscos e cuidados

- Alguns valores podem conter caracteres separadores no futuro. O formato novo resolve isso, mas o leitor legado ainda deve ser defensivo.
- `Filter.SetConjunto` hoje engole excecoes e aplica defaults. O parser novo deve retornar erro claro em testes, mas o leitor legado pode registrar aviso e manter fallback para nao quebrar arquivos antigos.
- `Arranjos` carrega dados de `LinPack.nfj` e do `acad.lin` no construtor. Separar isso do template para nao misturar catalogo de UI com configuracao salva.
- `Configuration.Config` global deve ser reduzido gradualmente. Migrar tudo de uma vez pode quebrar comandos da DLL.
- Manter cultura invariavel para numeros no XML estruturado (`.` como separador decimal).

## Criterio de pronto da primeira fase

A primeira fase esta pronta quando:

- O projeto contem `ConverterConfiguration` com `Arranjos` e blocos encapsulados.
- Um arquivo legado de `TemplatesAtivos` carrega para o modelo novo.
- O mesmo modelo salva em XML estruturado `VERSION="2"`.
- O XML estruturado carrega novamente.
- A API antiga ainda funciona atraves do mapper de compatibilidade.
