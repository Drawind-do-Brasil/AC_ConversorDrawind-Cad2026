using System;

namespace ConversorDrawind.Commands
{
    internal static class Localization
    {
        internal static string AppCopyright => ApplicationInfo.CopyrightText;
        internal static string AppDevelopedBy => $"Desenvolvido por {ApplicationInfo.DeveloperName}.";
        internal static string AppCompatibilityAutoCad => $"Compatível com {ApplicationInfo.AutoCadDisplayName}.";
        internal static string TitleError => "Erro";
        internal static string AlertErrorPrefix => "Erro:";

        internal static string MessageCompleted => "... Concluído.";
        internal static string MessageCompletedLowercase => "... concluído.";
        internal static string MessageFailedPrefix => "... erro.";
        internal static string MessageNoLogContext => "Falha sem contexto informado";
        internal static string MessageNoLogDetails => "Sem detalhes";
        internal static string MessageInternalConversionLogHeader => "Log de erros internos da conversão";
        internal static string MessageDrawingPrefix => "Desenho:";
        internal static string MessageAttributeListSizeMismatch => "A lista contém um número diferente de elementos.";
        internal static string MessageInvalidLinearDimensionEntity => "Não foi possível converter a cota linear: entidade inválida.";
        internal static string MessagePurgingDimensionStyles => "Purgando estilos de cota: ";
        internal static string MessagePurgingTextStyles => "Purgando estilos de texto: ";
        internal static string MessagePurgingLineTypes => "Purgando tipos de linha: ";
        internal static string MessagePurgingLayers => "Purgando layers: ";
        internal static string MessagePurgingBlocks => "Purgando blocos: ";
        internal static string MessageBlockNameRequired => "O nome do bloco deve ser informado.";
        internal static string MessageSourceDrawingRequired => "O arquivo DWG de origem deve ser informado.";

        internal static string PromptType => "Tipo: ";
        internal static string PromptClose => "Fechar: ";
        internal static string PromptBlockName => "Digite o nome do bloco: ";
        internal static string PromptSelectPoint => "Selecione um ponto: ";
        internal static string PromptSelectFirstPoint => "Selecione o primeiro ponto: ";
        internal static string PromptSelectSecondPoint => "\nSelecione o segundo ponto: ";
        internal static string PromptSelectObject => "Selecione um objeto: ";
        internal static string PromptSelectFirstDistancePoint => "Selecione o 1º ponto: ";
        internal static string PromptSelectSecondDistancePoint => "Selecione o 2º ponto: ";
        internal static string KeywordYes => "Sim";
        internal static string KeywordNo => "Não";

        internal static string StartExtractingBlocks => "Extraindo os blocos ";
        internal static string StartMoveToOrigin => "Movendo para a origem ";
        internal static string StartEditingNewBlock => "Editando o novo bloco... ";
        internal static string StartRemovingOldBlocks => "Removendo blocos antigos... ";
        internal static string StartRemovingUnusedLayers => "Removendo layers desnecessários... ";
        internal static string StartFixingDimensionArrows => "Corrigindo setas das cotas... ";
        internal static string StartPurgingDrawing => "Purgando desenho... ";
        internal static string StartCapturingFormatTexts => "Capturando textos do formato ";
        internal static string StartScalingDrawing => "Colocando o desenho na escala real... ";
        internal static string StartScalingFormat => "Colocando o formato na escala real... ";
        internal static string StartCreatingLayers => "Criando novos layers ";
        internal static string StartCreatingTextStyles => "Criando novos estilos de texto ";
        internal static string StartConvertingDimensions => "Convertendo as cotas ";
        internal static string StartExplodingBlocks => "Explodindo os blocos ";
        internal static string StartAddingDmBlock => "Adicionando bloco DM ";
        internal static string StartDeletingTeklaStructuresWord => "Excluindo a palavra \"Tekla Structures\" ";
        internal static string StartConvertingLayers => "Convertendo os layers ";

        internal static string ErrorEditingNewBlock => "Descrição: erro ao editar o novo bloco...";
        internal static string ErrorExtractingBlockLayers => "Descrição: erro ao extrair os layers dos blocos...";
        internal static string ErrorFixingDimensionArrows => "Descrição: erro ao tentar corrigir as setas das cotas...";
        internal static string ErrorPurgingDrawing => "Descrição: erro ao tentar purgar o desenho...";
        internal static string ErrorCapturingFormatTexts => "Descrição: erro ao capturar os textos no formato...";
        internal static string ErrorScalingDrawing => "Descrição: erro ao tentar colocar o desenho na escala real...";
        internal static string ErrorScalingFormat => "Descrição: erro ao tentar colocar o formato na escala real...";
        internal static string ErrorCreatingLayers => "Descrição: erro ao criar os novos layers...";
        internal static string ErrorCreatingTextStyles => "Descrição: erro ao criar os novos estilos de texto...";
        internal static string ErrorConvertingDimensions => "Descrição: erro ao converter as cotas...";
        internal static string ErrorExplodingBlocks => "Descrição: erro ao explodir os blocos...";
        internal static string ErrorAddingDmBlock => "Descrição: erro ao adicionar o bloco DM...";
        internal static string ErrorDeletingTeklaStructuresWord => "Descrição: erro ao excluir a palavra \"Tekla Structures\"...";
        internal static string ErrorConvertingLayers => "Descrição: erro ao converter os layers...";

        internal static string WarningCouldNotAttributeFormat(string blockNames) =>
            "Não foi possível atribuir o formato.\nOs blocos ou atributos dentro dos blocos não correspondem ao especificado.\nNomes dos blocos especificados: " + blockNames + ".";

        internal static string WarningCouldNotExtractBlockLayers => "Não foi possível extrair os layers dos blocos.\nVerifique se a conversão ocorreu normalmente.";
        internal static string WarningCouldNotPurgeDrawing => "Não foi possível remover layers, blocos e tipos de linha desnecessários.\nVerifique se a conversão ocorreu normalmente.";
        internal static string WarningCouldNotScaleDrawing => "Não foi possível colocar o desenho na escala real!";
        internal static string WarningCouldNotScaleFormat => "Não foi possível colocar o formato na escala real!";
        internal static string WarningCouldNotCreateLayers => "Não foi possível criar os novos layers.\nVerifique se a conversão ocorreu normalmente.";
        internal static string WarningCouldNotCreateTextStyles => "Não foi possível criar os novos estilos de texto.\nVerifique se a conversão ocorreu normalmente.";
        internal static string WarningCouldNotConvertDimensions => "Não foi possível converter as cotas.\nVerifique se a conversão ocorreu normalmente.";
        internal static string WarningCouldNotExplodeBlocks => "Não foi possível explodir os blocos.\nVerifique se a conversão ocorreu normalmente.";
        internal static string WarningCouldNotAddDmBlock => "Não foi possível adicionar o bloco DM.\nVerifique se a conversão ocorreu normalmente.";
        internal static string WarningCouldNotConvertLayers => "Não foi possível converter os layers.\nVerifique se a conversão ocorreu normalmente.";

        internal static string MessageConversionFinished => "Conversão finalizada.";
        internal static string FormatConversionSummary(string converterName, string userName, TimeSpan elapsed) =>
            "Conversão: " + converterName + "\tUsuário: " + userName + "\tTempo: " + elapsed.Hours + "h:" + elapsed.Minutes + "mm:" + elapsed.Seconds + "s:" + elapsed.Milliseconds + "ms";

        internal static string LogAlterarAtributosDoBloco => "Alterar atributos do bloco";
        internal static string LogAlterarAtributosRelacionados => "Alterar atributos relacionados do bloco";
        internal static string LogAplicarEstiloDeCota => "Aplicar estilo de cota";
        internal static string LogAtualizarConfiguracaoDaDimensao => "Atualizar configuração da cota";
        internal static string LogAtualizarPrecisaoDaCota => "Atualizar precisão da cota";
        internal static string LogAtualizarTextoDeCota => "Atualizar texto da cota";
        internal static string LogCapturarAtributosDosBlocos => "Capturar atributos dos blocos";
        internal static string LogCapturarDistanciaHorizontal => "Capturar distância horizontal";
        internal static string LogCapturarDistanciaVertical => "Capturar distância vertical";
        internal static string LogCapturarEscalaDoDesenho => "Capturar escala do desenho";
        internal static string LogCapturarInformacaoDoDesenho => "Capturar informação do desenho";
        internal static string LogCapturarLayer => "Capturar layer";
        internal static string LogCapturarPonto => "Capturar ponto";
        internal static string LogCapturarPontos => "Capturar pontos";
        internal static string LogCapturarTextosDoFormato => "Capturar textos do formato";
        internal static string LogCarregarConfiguracaoTemporaria => "Carregar configuração temporária";
        internal static string LogCarregarLayer => "Carregar layer";
        internal static string LogCarregarLinetype => "Carregar tipo de linha";
        internal static string LogConfigurarEstiloDeTexto => "Configurar estilo de texto";
        internal static string LogConverterBlocos => "Converter blocos";
        internal static string LogConverterCamadasIniciais => "Converter camadas iniciais";
        internal static string LogConverterCotas => "Converter cotas";
        internal static string LogConverterCotaAlinhada => "Converter cota alinhada";
        internal static string LogConverterCotaAngular => "Converter cota angular";
        internal static string LogConverterCotaDiametro => "Converter cota de diâmetro";
        internal static string LogConverterCotaLinear => "Converter cota linear";
        internal static string LogConverterCotaRadial => "Converter cota radial";
        internal static string LogConverterDesenho => "Converter desenho";
        internal static string LogConverterEntidadePorLayer => "Converter entidade por layer";
        internal static string LogConverterInstancia => "Converter instância";
        internal static string LogCriarBloco => "Criar bloco";
        internal static string LogCriarCamada => "Criar camada";
        internal static string LogCriarEstiloDeCota => "Criar estilo de cota";
        internal static string LogCriarEstilosDeTexto => "Criar estilos de texto";
        internal static string LogDefinirEscalaDoBloco => "Definir escala do bloco";
        internal static string LogEditarNovoBloco => "Editar novo bloco";
        internal static string LogExplodirBlocos => "Explodir blocos";
        internal static string LogExplodirBlocosEMTextos => "Explodir blocos e textos";
        internal static string LogExplodirCotasImportadas => "Explodir cotas importadas";
        internal static string LogExplodirCotaRadialGrande => "Explodir cota radial grande";
        internal static string LogExplodirEntidade => "Explodir entidade";
        internal static string LogExplodirTiposConhecidos => "Explodir tipos conhecidos";
        internal static string LogFinalizarConversao => "Finalizar conversão";
        internal static string LogFixarSetaDaCota => "Corrigir seta da cota";
        internal static string LogLimparCamadasTekla => "Limpar camadas Tekla";
        internal static string LogMoverDesenhoParaOrigem => "Mover desenho para origem";
        internal static string LogPrepararConversao => "Preparar conversão";
        internal static string LogPublicarArquivo => "Publicar arquivo";
        internal static string LogRemoverBloco => "Remover bloco";
        internal static string LogRemoverBlocos => "Remover blocos";
        internal static string LogRemoverCamada => "Remover camada";
        internal static string LogRemoverCamadas => "Remover camadas";
        internal static string LogRemoverCamadasDesnecessarias => "Remover camadas desnecessárias";
        internal static string LogRemoverReferenciasDeTabela => "Remover referências de tabela";
        internal static string LogSalvarDesenho => "Salvar desenho";
        internal static string LogSalvarDxf => "Salvar DXF";
        internal static string LogSelecionarEntidadesPorLayer => "Selecionar entidades por layer";
        internal static string LogSelecionarLayers => "Selecionar layers";
        internal static string LogVerificarBlocoRelacionado => "Verificar bloco relacionado";
        internal static string LogZoomNoDesenho => "Aplicar zoom no desenho";

        internal static string FormScaleTitle => "Escala";
        internal static string FormScalePrompt => "Informe a escala do desenho: ";
        internal static string ButtonOk => "OK";
        internal static string ButtonCancel => "Cancelar";
    }
}
