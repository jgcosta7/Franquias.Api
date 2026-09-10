# Sistema de Gestão de Franquias

API REST desenvolvida em C# com ASP.NET Core Web API para gerenciamento
de uma rede de franquias.

O sistema permite o gerenciamento de usuários, perfis, franqueadoras,
unidades franqueadas, categorias, produtos e serviços, estoque, vendas,
fornecedores, royalties e chamados de suporte.

## Tecnologias utilizadas

-   C#
-   ASP.NET Core Web API
-   Entity Framework Core
-   SQLite
-   JWT (JSON Web Token)
-   BCrypt
-   Swagger / OpenAPI
-   Git e GitHub

## Funcionalidades

### Usuários e autenticação

-   Cadastro de usuários
-   Cadastro de perfis
-   Controle de usuários ativos e inativos
-   Validação de e-mail duplicado
-   Senhas armazenadas com hash BCrypt
-   Login utilizando JWT
-   Proteção de endpoints com `[Authorize]`

### Franqueadoras e unidades

-   Cadastro de franqueadoras
-   Cadastro de unidades franqueadas
-   Associação entre unidade e franqueadora
-   Validação de CNPJ duplicado
-   Controle de unidades ativas e inativas

### Produtos e categorias

-   Cadastro de categorias
-   Cadastro de produtos e serviços
-   Associação de produtos a categorias
-   Controle de produtos ativos e inativos
-   Validação de preço

### Estoque

-   Cadastro de estoque por unidade e produto
-   Entrada de estoque
-   Saída de estoque
-   Registro de movimentações
-   Controle de estoque mínimo
-   Bloqueio de saída quando não há quantidade suficiente
-   O estoque não pode ficar negativo

### Vendas

-   Registro de vendas
-   Associação da venda a uma unidade
-   Inclusão de produtos na venda
-   Cálculo automático do subtotal dos itens
-   Cálculo automático do valor total da venda
-   Baixa automática no estoque
-   Venda exige pelo menos um item
-   Unidade inativa não pode realizar vendas

### Fornecedores

-   Cadastro de fornecedores
-   Consulta de fornecedores
-   Atualização de fornecedores
-   Desativação de fornecedores
-   Validação de CNPJ duplicado

### Royalties

-   Cálculo de royalties por unidade
-   Definição do percentual de royalty
-   Cálculo baseado no faturamento do período
-   Consulta de royalties
-   Controle de status de pagamento
-   Registro da data de pagamento

### Chamados de suporte

-   Abertura de chamados
-   Associação do chamado a uma unidade
-   Categoria e prioridade
-   Controle de status
-   Encerramento de chamados

## Relatórios

A API possui endpoints específicos para consulta de indicadores e
relatórios:

-   Unidades ativas e inativas
-   Estoque e produtos com estoque baixo
-   Vendas por período
-   Faturamento por unidade
-   Produtos mais vendidos
-   Chamados em aberto

## Estrutura do projeto

``` text
Franquias.Api/
│
├── Controllers/
│   ├── AuthController.cs
│   ├── CategoriaController.cs
│   ├── ChamadoController.cs
│   ├── EstoqueController.cs
│   ├── FornecedorController.cs
│   ├── FranqueadoraController.cs
│   ├── PerfilController.cs
│   ├── ProdutoController.cs
│   ├── RelatorioController.cs
│   ├── RoyaltyController.cs
│   ├── UnidadeController.cs
│   ├── UsuarioController.cs
│   └── VendaController.cs
│
├── Data/
│   ├── ApplicationDbContext.cs
│   └── SeedData.cs
│
├── DTOs/
│   ├── LoginDto.cs
│   ├── MovimentacaoEstoqueDto.cs
│   ├── UsuarioCadastroDto.cs
│   └── VendaDto.cs
│
├── Models/
│   ├── Perfil.cs
│   ├── Usuario.cs
│   ├── Franqueadora.cs
│   ├── UnidadeFranqueada.cs
│   ├── Categoria.cs
│   ├── ProdutoServico.cs
│   ├── Fornecedor.cs
│   ├── Estoque.cs
│   ├── MovimentacaoEstoque.cs
│   ├── Venda.cs
│   ├── ItemVenda.cs
│   ├── Royalty.cs
│   └── ChamadoSuporte.cs
│
├── Migrations/
├── Configurations/
├── Services/
├── Repositories/
│
├── appsettings.json
├── Program.cs
└── README.md
```

## Banco de dados

O projeto utiliza SQLite com Entity Framework Core.

A conexão está configurada em:

``` json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=franquias.db"
  }
}
```

## Como executar o projeto

### Pré-requisitos

É necessário ter instalado:

-   .NET SDK
-   Visual Studio Code ou outra IDE compatível
-   Git

### Clonar o projeto

``` bash
git clone URL_DO_REPOSITORIO
```

Entrar na pasta:

``` bash
cd Franquias.Api
```

### Restaurar as dependências

``` bash
dotnet restore
```

### Criar/atualizar o banco

``` bash
dotnet ef database update
```

### Executar a API

``` bash
dotnet run
```

A API será disponibilizada em:

``` text
http://localhost:5220
```

## Swagger

Após iniciar a aplicação, a documentação interativa pode ser acessada
em:

``` text
http://localhost:5220/swagger
```

O Swagger permite visualizar e testar os endpoints da API.

## Autenticação

O sistema utiliza JWT para autenticação.

### Login

Endpoint:

``` http
POST /api/Auth/login
```

Exemplo:

``` json
{
  "email": "admin@franquias.com",
  "senha": "123456"
}
```

A API retorna um token JWT.

Exemplo de resposta:

``` json
{
  "token": "SEU_TOKEN_JWT",
  "usuario": "Administrador",
  "perfil": "Administrador"
}
```

### Utilizando o token

No Swagger:

1.  Realize o login.
2.  Copie o token retornado.
3.  Clique em **Authorize**.
4.  Informe:

``` text
Bearer SEU_TOKEN_JWT
```

5.  Confirme a autenticação.

Endpoints protegidos exigem um token JWT válido.

## Principais endpoints

### Autenticação

``` text
POST /api/Auth/login
```

### Perfis

``` text
GET    /api/Perfil
GET    /api/Perfil/{id}
POST   /api/Perfil
PUT    /api/Perfil/{id}
DELETE /api/Perfil/{id}
```

### Usuários

``` text
GET    /api/Usuario
GET    /api/Usuario/{id}
POST   /api/Usuario
DELETE /api/Usuario/{id}
```

### Franqueadoras

``` text
GET    /api/Franqueadora
GET    /api/Franqueadora/{id}
POST   /api/Franqueadora
PUT    /api/Franqueadora/{id}
DELETE /api/Franqueadora/{id}
```

### Unidades

``` text
GET    /api/Unidade
GET    /api/Unidade/{id}
POST   /api/Unidade
PUT    /api/Unidade/{id}
DELETE /api/Unidade/{id}
```

### Categorias

``` text
GET  /api/Categoria
POST /api/Categoria
```

### Produtos

``` text
GET  /api/Produto
GET  /api/Produto/{id}
POST /api/Produto
```

### Estoque

``` text
GET  /api/Estoque
POST /api/Estoque
POST /api/Estoque/movimentar
```

### Vendas

``` text
GET  /api/Venda
POST /api/Venda
```

### Fornecedores

``` text
GET    /api/Fornecedor
GET    /api/Fornecedor/{id}
POST   /api/Fornecedor
PUT    /api/Fornecedor/{id}
DELETE /api/Fornecedor/{id}
```

### Royalties

``` text
GET  /api/Royalty
GET  /api/Royalty/{id}
POST /api/Royalty/calcular
PUT  /api/Royalty/{id}/pagar
```

### Chamados

``` text
GET /api/Chamado
POST /api/Chamado
PUT /api/Chamado/{id}/encerrar
```

### Relatórios

``` text
GET /api/Relatorio/unidades
GET /api/Relatorio/estoque
GET /api/Relatorio/vendas
GET /api/Relatorio/faturamento
GET /api/Relatorio/produtos-mais-vendidos
GET /api/Relatorio/chamados-abertos
```

## Regras de negócio

O sistema possui validações para garantir a integridade dos dados.

### Usuários

-   Não permite dois usuários com o mesmo e-mail.
-   O usuário precisa estar vinculado a um perfil válido e ativo.
-   A senha é armazenada utilizando hash BCrypt.

### Unidades

-   Não permite duas unidades com o mesmo CNPJ.
-   A unidade precisa estar vinculada a uma franqueadora válida e ativa.
-   Unidades podem ser desativadas sem exclusão física do registro.

### Produtos

-   O produto precisa possuir nome.
-   O preço base deve ser maior que zero.
-   A categoria precisa existir e estar ativa.

### Estoque

-   A quantidade não pode ser negativa.
-   Não é permitida saída maior que o estoque disponível.
-   Cada unidade possui apenas um registro de estoque para cada produto.
-   As movimentações de entrada e saída são registradas.

### Vendas

-   Toda venda precisa possuir pelo menos um item.
-   A unidade precisa estar ativa.
-   Os produtos precisam estar ativos.
-   Deve existir estoque suficiente.
-   O subtotal dos itens é calculado automaticamente.
-   O valor total da venda é calculado automaticamente.
-   O estoque é reduzido após o registro da venda.

### Royalties

-   O percentual deve estar entre 0 e 100.
-   A data inicial não pode ser maior que a data final.
-   O cálculo considera somente vendas confirmadas.
-   O valor do royalty é calculado com base no faturamento do período.

### Chamados

-   O chamado precisa possuir descrição.
-   A unidade vinculada precisa existir e estar ativa.
-   Chamados podem ser encerrados posteriormente.

## Dados de exemplo

O projeto possui um mecanismo de Seed em:

``` text
Data/SeedData.cs
```

Ele permite inicializar dados de exemplo para facilitar testes e
demonstrações.

Os dados incluem exemplos de:

-   Perfis
-   Usuários
-   Franqueadora
-   Unidades
-   Categorias
-   Produtos
-   Fornecedor
-   Estoque
-   Vendas
-   Chamados
-   Royalties

O Seed verifica se já existem usuários no banco antes de inserir os
dados, evitando duplicações.

## Migrations

As migrations do Entity Framework Core ficam armazenadas na pasta:

``` text
Migrations/
```

Para criar uma nova migration:

``` bash
dotnet ef migrations add NomeDaMigration
```

Para atualizar o banco:

``` bash
dotnet ef database update
```

## Testes

Os principais testes podem ser realizados diretamente pelo Swagger.

Exemplos de cenários:

### Login válido

``` text
POST /api/Auth/login
→ 200 OK
```

### Login inválido

``` text
POST /api/Auth/login
→ 401 Unauthorized
```

### E-mail duplicado

``` text
POST /api/Usuario
→ 409 Conflict
```

### CNPJ duplicado

``` text
POST /api/Unidade
→ 409 Conflict
```

### Estoque insuficiente

``` text
POST /api/Estoque/movimentar
→ 400 Bad Request
```

### Venda sem itens

``` text
POST /api/Venda
→ 400 Bad Request
```

### Acesso a rota protegida sem JWT

``` text
GET /api/Usuario
→ 401 Unauthorized
```

### Acesso a rota protegida com JWT

``` text
GET /api/Usuario
→ 200 OK
```

## Considerações

O projeto foi desenvolvido com foco em uma API REST simples, utilizando
Entity Framework Core para persistência dos dados e JWT para
autenticação.

A estrutura prioriza simplicidade e organização, mantendo as regras de
negócio próximas às operações realizadas pela API.

Em um ambiente de produção, seria recomendado utilizar variáveis de
ambiente ou mecanismos seguros de configuração para armazenar a chave
JWT, além de ampliar a cobertura de testes automatizados e o controle de
permissões por perfil.

## Autor: João Gabriel

Este projeto acadêmico foi desenvolvido para a disciplina de Desenvolvimento
Back-end.

Tecnologias principais:

C# \| ASP.NET Core \| Entity Framework Core \| SQLite \| JWT \| Swagger
