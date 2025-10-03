using System.Diagnostics.CodeAnalysis;

namespace API_Miniapp_Gestao.Constants;

[ExcludeFromCodeCoverage]
public static class DatabaseConstants
{
    // Nomes de propriedades para Foreign Keys
    public static class ForeignKeyProperties
    {
        public const string CoVersao = "CoVersao";
        public const string CoVersaoAplicativo = "CoVersaoAplicativo";
        public const string CoVersaoMiniapp = "CoVersaoMiniapp";
        public const string CoModeloDevice = "CoModeloDevice";
        public const string CoVersaoSistemaOperacional = "CoVersaoSistemaOperacional";
    }

    // Nomes das Tabelas
    public static class TableNames
    {
        public const string NBMTB001_VERSAO_SISTEMA_OPERACIONAL = "NBMTB001_VERSAO_SISTEMA_OPERACIONAL";
        public const string NBMTB002_MODELO_DEVICE = "NBMTB002_MODELO_DEVICE";
        public const string NBMTB003_VERSAO_APLICATIVO = "NBMTB003_VERSAO_APLICATIVO";
        public const string NBMTB004_MINIAPP = "NBMTB004_MINIAPP";
        public const string NBMTB005_RELACIONAMENTO_MINIAPP = "NBMTB005_RELACIONAMENTO_MINIAPP";
        public const string NBMTB006_VERSAO_MINIAPP = "NBMTB006_VERSAO_MINIAPP";
        public const string NBMTB007_VERSAO_MINIAPP_VS_SISTEMA_OPERACIONAL = "NBMTB007_VERSAO_MINIAPP_VS_SISTEMA_OPERACIONAL";
        public const string NBMTB008_VERSAO_MINIAPP_VS_APLICATIVO = "NBMTB008_VERSAO_MINIAPP_VS_APLICATIVO";
        public const string NBMTB009_VERSAO_MINIAPP_MODELO_DEVICE = "NBMTB009_VERSAO_MINIAPP_MODELO_DEVICE";
        public const string NBMTB010_USUARIO_VERSAO_MINIAPP = "NBMTB010_USUARIO_VERSAO_MINIAPP";
    }

    // Nomes das Colunas
    public static class ColumnNames
    {
        // Colunas comuns
        public const string CO_VERSAO_MINIAPP = "CO_VERSAO_MINIAPP";
        public const string CO_MINIAPP = "CO_MINIAPP";
        public const string CO_PLATAFORMA = "CO_PLATAFORMA";
        public const string IC_ATIVO = "IC_ATIVO";

        // Colunas específicas da NBMTB001
        public const string CO_VERSAO_SISTEMA_OPERACIONAL = "CO_VERSAO_SISTEMA_OPERACIONAL";
        public const string NU_VERSAO_SISTEMA_OPERACIONAL = "NU_VERSAO_SISTEMA_OPERACIONAL";

        // Colunas específicas da NBMTB002
        public const string CO_MODELO_DEVICE = "CO_MODELO_DEVICE";
        public const string DE_MODELO_DEVICE = "DE_MODELO_DEVICE";

        // Colunas específicas da NBMTB003
        public const string CO_VERSAO_APLICATIVO = "CO_VERSAO_APLICATIVO";
        public const string NU_VERSAO_APLICATIVO = "NU_VERSAO_APLICATIVO";

        // Colunas específicas da NBMTB004
        public const string DE_MINIAPP = "DE_MINIAPP";
        public const string IC_MINIAPP_INICIAL = "IC_MINIAPP_INICIAL";
        public const string NO_APELIDO_MINIAPP = "NO_APELIDO_MINIAPP";
        public const string NO_MINIAPP = "NO_MINIAPP";

        // Colunas específicas da NBMTB005
        public const string CO_MINIAPP_PAI = "CO_MINIAPP_PAI";
        public const string CO_MINIAPP_FILHO = "CO_MINIAPP_FILHO";

        // Colunas específicas da NBMTB006
        public const string DH_FIM_VIGENCIA = "DH_FIM_VIGENCIA";
        public const string DH_INICIO_VIGENCIA = "DH_INICIO_VIGENCIA";
        public const string ED_VERSAO_MINIAPP = "ED_VERSAO_MINIAPP";
        public const string NU_VERSAO_MINIAPP = "NU_VERSAO_MINIAPP";
        public const string PC_EXPANSAO_MINIAPP = "PC_EXPANSAO_MINIAPP";

        // Colunas específicas da NBMTB010
        public const string CO_USUARIO_VERSAO_MINIAPP = "CO_USUARIO_VERSAO_MINIAPP";
        public const string NU_CPF_USUARIO = "NU_CPF_USUARIO";
    }

    // Nomes dos Índices
    public static class IndexNames
    {
        public const string IX_NBMTB002_MODELO_DEVICE = "IX_NBMTB002_MODELO_DEVICE";
        public const string IX_NBMTB003_VERSAO_APLICATIVO = "IX_NBMTB003_VERSAO_APLICATIVO";
        public const string IX_NBMTB004_MINIAPP = "IX_NBMTB004_MINIAPP";
        public const string IX_NBMTB006_VERSAO_MINIAPP = "IX_NBMTB006_VERSAO_MINIAPP";
        public const string IX_NBMTB010_USUARIO_VERSAO_MINIAPP = "IX_NBMTB010_USUARIO_VERSAO_MINIAPP";
    }

    // Nomes das Foreign Keys
    public static class ForeignKeyNames
    {
        public const string FK_NBMTB005_RELACIONAMENTO_MINIAPP_NBMTB004_MINIAPP_PAI = "FK_NBMTB005_RELACIONAMENTO_MINIAPP_NBMTB004_MINIAPP_PAI";
        public const string FK_NBMTB005_RELACIONAMENTO_MINIAPP_NBMTB004_MINIAPP_FILHO = "FK_NBMTB005_RELACIONAMENTO_MINIAPP_NBMTB004_MINIAPP_FILHO";
        public const string FK_NBMTB006_VERSAO_MINIAPP_NBMTB004_MINIAPP = "FK_NBMTB006_VERSAO_MINIAPP_NBMTB004_MINIAPP";
        public const string FK_NBMTB007_VS_MINIAPP_VS_SISTEMA_OPERACIONAL_NBMTB001_VERSAO_SISTEMA_OPERACIONAL = "FK_NBMTB007_VS_MINIAPP_VS_SISTEMA_OPERACIONAL_NBMTB001_VERSAO_SISTEMA_OPERACIONAL";
        public const string FK_NBMTB007_VS_MINIAPP_VS_SISTEMA_OPERACIONAL_NBMTB006_VERSAO_MINIAPP = "FK_NBMTB007_VS_MINIAPP_VS_SISTEMA_OPERACIONAL_NBMTB006_VERSAO_MINIAPP";
        public const string FK_NBMTB008_VS_MINIAPP_VS_APLICATIVO_NBMTB003_VERSAO_APLICATIVO = "FK_NBMTB008_VS_MINIAPP_VS_APLICATIVO_NBMTB003_VERSAO_APLICATIVO";
        public const string FK_NBMTB008_VS_MINIAPP_VS_APLICATIVO_NBMTB006_VERSAO_MINIAPP = "FK_NBMTB008_VS_MINIAPP_VS_APLICATIVO_NBMTB006_VERSAO_MINIAPP";
        public const string FK_NBMTB009_VS_MINIAPP_MODELO_DEVICE_NBMTB002_MODELO_DEVICE = "FK_NBMTB009_VS_MINIAPP_MODELO_DEVICE_NBMTB002_MODELO_DEVICE";
        public const string FK_NBMTB009_VS_MINIAPP_MODELO_DEVICE_NBMTB006_VERSAO_MINIAPP = "FK_NBMTB009_VS_MINIAPP_MODELO_DEVICE_NBMTB006_VERSAO_MINIAPP";
        public const string FK_NBMTB010_USUARIO_VS_MINIAPP_NBMTB006_VERSAO_MINIAPP = "FK_NBMTB010_USUARIO_VS_MINIAPP_NBMTB006_VERSAO_MINIAPP";
    }
}
