# 🧪 Testes Unitários - API Versão MiniApp

Este documento descreve os testes unitários implementados para os métodos `CriarVersaoMiniapp` e `ListarVersoesMiniapp` em todas as camadas da aplicação.

## ✅ **STATUS: TODOS OS TESTES PASSANDO (55/55)**

**Resumo do teste:** Total: 55 | Aprovados: 55 | Falharam: 0 | Ignorados: 0

---

## 📁 Estrutura dos Testes

```
API_Miniapp_Gestao.Tests/
├── Base/
│   └── TestBase.cs                               # Classe base para configuração comum
├── Controllers/
│   └── VersaoMiniappControllerTests.cs          # Testes do Controller
├── Services/
│   └── MiniappVersaoServiceTests.cs             # Testes do Service
├── Repositories/
│   └── MiniAppVersaoRepositoryTests.cs          # Testes do Repository
└── Integration/
    └── CriarVersaoMiniappIntegrationTests.cs    # Testes de Integração
```

## 🎯 Cenários de Teste Cobertos

### 📊 **Repository (MiniAppVersaoRepositoryTests)**

#### ✅ Cenários de Sucesso:
- `CriarVersaoMiniappAsync_ComEntradaValida_DeveCriarVersaoComSucesso`
- `CriarVersaoMiniappAsync_ComValoresNulos_DeveUsarValoresPadrao`
- `CriarVersaoMiniappAsync_DeveGerarCoVersaoUnico`
- `CriarVersaoMiniappAsync_DeveLogOperacoes`

#### ❌ Cenários de Erro:
- `CriarVersaoMiniappAsync_ComMiniappInexistente_DeveLancarKeyNotFoundException`

### 🛠️ **Service (MiniappVersaoServiceTests)**

#### ✅ Cenários de Sucesso (Criar):
- `CriarVersaoMiniappAsync_ComEntradaValida_DeveCriarVersaoComSucesso`
- `CriarVersaoMiniappAsync_DeveLogOperacoes`
- `CriarVersaoMiniappAsync_ComValoresOpcionaisNulos_DeveProcessarCorretamente` (Theory)

#### ❌ Cenários de Erro (Criar):
- `CriarVersaoMiniappAsync_QuandoRepositorioLancaKeyNotFoundException_DeveRepassarExcecao`
- `CriarVersaoMiniappAsync_QuandoRepositorioLancaException_DeveRepassarExcecao`
- `CriarVersaoMiniappAsync_QuandoOcorreErro_DeveLogErro`

#### ✅ Cenários de Sucesso (Listar):
- `ListarVersoesMiniappAsync_ComEntradaValida_DeveRetornarListaVersoes`
- `ListarVersoesMiniappAsync_DeveLogOperacoes`
- `ListarVersoesMiniappAsync_ComDiferentesGUIDs_DeveProcessarCorretamente` (Theory)

#### ❌ Cenários de Erro (Listar):
- `ListarVersoesMiniappAsync_ComCoMiniappNulo_DeveLancarArgumentException`
- `ListarVersoesMiniappAsync_ComCoMiniappVazio_DeveLancarArgumentException`
- `ListarVersoesMiniappAsync_ComCoMiniappInvalido_DeveLancarArgumentException`
- `ListarVersoesMiniappAsync_QuandoRepositorioLancaKeyNotFoundException_DeveRepassarExcecao`
- `ListarVersoesMiniappAsync_QuandoRepositorioLancaException_DeveRepassarExcecao`
- `ListarVersoesMiniappAsync_QuandoOcorreErro_DeveLogErro`

### 🎮 **Controller (VersaoMiniappControllerTests)**

#### ✅ Cenários de Sucesso (Criar):
- `PostVersaoMiniapp_ComEntradaValida_DeveRetornarCreatedComDadosCorretos`
- `PostVersaoMiniapp_ComDiferentesValoresOpcionais_DeveProcessarCorretamente` (Theory)
- `PostVersaoMiniapp_DeveLogOperacoes`

#### ❌ Cenários de Erro (Criar):
- `PostVersaoMiniapp_ComModelStateInvalido_DeveRetornarBadRequest`
- `PostVersaoMiniapp_ComBusinessExceptionNotFound_DeveRetornarNotFound`
- `PostVersaoMiniapp_ComBusinessExceptionConflict_DeveRetornarConflict`
- `PostVersaoMiniapp_ComBusinessExceptionBadRequest_DeveRetornarBadRequest`
- `PostVersaoMiniapp_ComExceptionGenerica_DeveRetornarInternalServerError`

#### ✅ Cenários de Sucesso (Listar):
- `GetVersoesMiniapps_ComGuidValido_DeveRetornarOkComListaVersoes`
- `GetVersoesMiniapps_ComListaVazia_DeveRetornarOkComListaVazia`
- `GetVersoesMiniapps_ComDiferentesValoresCoMiniapp_DeveProcessarCorretamente` (Theory)

#### ❌ Cenários de Erro (Listar):
- `GetVersoesMiniapps_ComModelStateInvalido_DeveRetornarBadRequest`
- `GetVersoesMiniapps_ComArgumentException_DeveRetornarBadRequest`
- `GetVersoesMiniapps_ComKeyNotFoundException_DeveRetornarNotFound`
- `GetVersoesMiniapps_ComExceptionGenerica_DeveRetornarInternalServerError`

#### 📝 Cenários de Log (Listar):
- `GetVersoesMiniapps_DeveLogOperacoes`
- `GetVersoesMiniapps_ComArgumentException_DeveLogWarning`
- `GetVersoesMiniapps_ComKeyNotFoundException_DeveLogWarning`
- `GetVersoesMiniapps_ComExceptionGenerica_DeveLogError`

### 🔄 **Integração (CriarVersaoMiniappIntegrationTests)**

#### ✅ Fluxo Completo:
- `FluxoCompleto_CriarVersaoMiniapp_DeveProcessarComSucesso`
- `FluxoCompleto_CriarMultiplasVersoes_DeveGerarCodigosUnicos`
- `FluxoCompleto_CriarVersaoComValoresOpcionais_DeveProcessarCorretamente` (Theory)

#### ❌ Cenários de Erro:
- `FluxoCompleto_CriarVersaoMiniappInexistente_DeveRetornarNotFound`

## 🧩 Dados de Teste

### 📝 Entrada Padrão para Criar Versão:
```csharp
var entradaCriarVersao = new EntradaCriarVersaoMiniappDto
{
    CoMiniapp = coMiniapp,
    NuVersao = 1.5m,
    PcExpansao = 10.0m,
    IcAtivo = true,
    EdVersaoMiniapp = "1.5.0"
};
```

### 📤 Retorno Esperado (Criar):
```csharp
var retornoEsperado = new RetornoCriarVersaoMiniappDto
{
    CoVersao = Guid.NewGuid(),
    CoMiniapp = coMiniapp,
    NuVersao = 1.5m,
    PcExpansao = 10.0m,
    IcAtivo = true,
    EdVersaoMiniapp = "1.5.0"
};
```

### 📝 Entrada Padrão para Listar Versões:
```csharp
var entradaListar = new EntradaMiniappDto
{
    CoMiniapp = "123e4567-e89b-12d3-a456-426614174000"
};
```

### 📤 Retorno Esperado (Listar):
```csharp
var retornoEsperado = new RetornoListarVersoesDto
{
    RetornoListaVersoes = new List<VersaoMiniappDto>
    {
        new() { CoVersao = Guid.NewGuid(), NuVersao = 1.0m, IcAtivo = true },
        new() { CoVersao = Guid.NewGuid(), NuVersao = 2.0m, IcAtivo = false }
    }
};
```

## 🚀 Como Executar os Testes

### Executar Todos os Testes:
```bash
cd "c:\desenvolvimento\dotnet\API_Miniapp_Gestao"
dotnet test
```

### Executar Testes com Detalhes:
```bash
dotnet test --logger "console;verbosity=detailed"
```

### Executar Testes do Controller de Versão:
```bash
dotnet test --filter "ClassName=VersaoMiniappControllerTests"
```

### Executar Apenas Testes de Listagem:
```bash
dotnet test --filter "MethodName~GetVersoesMiniapps"
```

### Executar Apenas Testes de Criação:
```bash
dotnet test --filter "MethodName~PostVersaoMiniapp"
```

### Executar um Teste Específico:
```bash
dotnet test --filter "MethodName=CriarVersaoMiniappAsync_ComEntradaValida_DeveCriarVersaoComSucesso"
```

## 📋 Validações Realizadas

### ✅ **Validações de Dados**:
- Verificação de campos obrigatórios
- Validação de tipos de dados
- Tratamento de valores nulos
- Valores padrão aplicados corretamente

### ✅ **Validações de Negócio**:
- Miniapp deve existir antes de criar versão
- CoVersao deve ser único
- Logs devem ser gerados corretamente
- Exceções devem ser tratadas adequadamente

### ✅ **Validações de Resposta HTTP**:
- **Criar Versão**: Status codes corretos (201, 400, 404, 409, 500)
- **Listar Versões**: Status codes corretos (200, 400, 404, 500)
- Estrutura de resposta correta
- Headers apropriados (Location no Created)
- Mensagens de erro informativas
- Retorno de lista vazia quando aplicável

## 🛡️ Mocks e Dependências

### 🔧 **Dependências Mockadas**:
- `ILogger<T>` - Para validar logs
- `IMiniAppVersaoRepository` - Para testes do service
- `IMiniappVersaoService` - Para testes do controller

### 💾 **Banco de Dados**:
- Entity Framework InMemory Database
- Instâncias únicas por teste (isolamento)
- Limpeza automática após cada teste

## 📊 Cobertura de Código

Os testes cobrem:
- ✅ Todos os cenários de sucesso
- ✅ Todos os cenários de erro conhecidos
- ✅ Validações de entrada
- ✅ Comportamentos de log
- ✅ Integração entre camadas
- ✅ Casos extremos (valores nulos, GUIDs inválidos)

## 🔍 Padrões Utilizados

### **AAA Pattern**:
- **Arrange**: Configuração dos dados e mocks
- **Act**: Execução do método testado
- **Assert**: Verificação dos resultados

### **Nomenclatura**:
- `NomeDoMetodo_Cenario_ComportamentoEsperado`
- Ex: `CriarVersaoMiniappAsync_ComEntradaValida_DeveCriarVersaoComSucesso`

### **Teorias (Theory)**:
- Usado para testar múltiplos cenários com dados diferentes
- Evita duplicação de código
- Melhora a cobertura de casos extremos

## 🔧 **Correções Aplicadas**

### **1. Problema de Dependências NuGet**
- ❌ **Erro**: Downgrade de pacote `Microsoft.Extensions.Logging.Abstractions`
- ✅ **Solução**: Removido referência explícita, usando versão transitiva correta

### **2. Problema de Conversão Decimal/Double**
- ❌ **Erro**: `System.Double` cannot be converted to `System.Nullable<System.Decimal>`
- ✅ **Solução**: Alterado testes [Theory] para usar strings e conversão com `CultureInfo.InvariantCulture`

### **3. Problema de Valores Padrão**
- ❌ **Erro**: Repository retornando `entrada.NuVersao` em vez de valor processado
- ✅ **Solução**: Corrigido retorno do repository para usar `novaVersao.NuVersaoMiniapp`

### **4. Problema de Persistência**
- ❌ **Erro**: DbEscrita e DbLeitura usando bancos diferentes
- ✅ **Solução**: Configurado mesmo banco em memória para ambos os contextos

### **5. Problema de URL vs Versão**
- ❌ **Erro**: Campo `EdVersaoMiniapp` tratado como número em vez de URL
- ✅ **Solução**: Implementado geração automática de URL padrão e ajustado testes

### **6. Problema de Tratamento de Exceções**
- ❌ **Erro**: `KeyNotFoundException` sendo capturada como exceção genérica
- ✅ **Solução**: Adicionado tratamento específico no controller

---

💡 **Dica**: Execute `dotnet test --collect:"XPlat Code Coverage"` para gerar relatórios de cobertura de código.
