
# 1 Introdução 
A adoção de microserviços combinada com Hexagonal Architecture, Domain‑Driven Design (DDD), Test‑Driven Development (TDD), Command Query Responsibility Segregation (CQRS) e os princípios SOLID estabelece uma base arquitetural robusta para sistemas distribuídos modernos. Essa abordagem promove modularidade, testabilidade, independência de implantação e alinhamento do software ao domínio do negócio.

## 1.2 Conceitos Fundamentais  
### 1.2.1 Microserviços  
Microserviços são unidades independentes e autônomas que encapsulam funcionalidades específicas do domínio. Cada serviço possui seu próprio ciclo de vida, banco de dados e pipeline de entrega.

### 1.2.2 Hexagonal Architecture  
A Arquitetura Hexagonal (Ports and Adapters) separa o núcleo de domínio das interfaces externas.  
- Ports: contratos que definem como o domínio interage com o mundo externo.  
- Adapters: implementações concretas desses contratos.  
- Domain Core: regras de negócio puras, independentes de frameworks.

### 1.2.3 Domain‑Driven Design (DDD)  
DDD organiza o software em torno do domínio do negócio.  
Principais elementos:  
- Entities  
- Value Objects  
- Aggregates  
- Domain Services  
- Repositories  
- Bounded Contexts  

### 1.2.4 Test‑Driven Development (TDD)  
TDD segue o ciclo Red → Green → Refactor, garantindo que o design seja guiado por testes e que o domínio permaneça desacoplado.

### 1.2.5 CQRS  
CQRS separa operações de escrita (Commands) das operações de leitura (Queries), permitindo otimizações específicas para cada tipo de operação.

### 1.2.6 SOLID  
Princípios que orientam a construção de componentes coesos e de baixo acoplamento:  
- Single Responsibility  
- Open/Closed  
- Liskov Substitution  
- Interface Segregation  
- Dependency Inversion  

## 1.3 Estrutura Arquitetural  
### 1.3.1 Camada de Domínio  
Contém regras de negócio puras, sem dependências externas.

### 1.3.2 Camada de Aplicação  
Orquestra casos de uso, coordena comandos e queries, e interage com o domínio por meio de ports.

### 1.3.3 Camada de Infraestrutura  
Implementa adapters, persistência, mensageria, APIs externas e configurações.

### 1.3.4 Comunicação entre Microserviços  
Pode ocorrer via eventos assíncronos, REST ou gRPC, dependendo do contexto.

## 1.4 Sintaxe e Estrutura de Código  
### 1.4.1 Exemplo de Entidade (DDD)  
```csharp
public class Pedido
{
    public Guid Id { get; }
    public DateTime DataCriacao { get; }
    public IReadOnlyCollection<ItemPedido> Itens => _itens.AsReadOnly();

    private readonly List<ItemPedido> _itens = new();

    public Pedido(Guid id)
    {
        Id = id;
        DataCriacao = DateTime.UtcNow;
    }

    public void AdicionarItem(string descricao, int quantidade)
    {
        if (quantidade <= 0)
            throw new ArgumentException("Quantidade inválida.");

        _itens.Add(new ItemPedido(descricao, quantidade));
    }
}
```

### 1.4.2 Exemplo de Value Object  
```csharp
public record ItemPedido(string Descricao, int Quantidade);
```

### 1.4.3 Exemplo de Port (Hexagonal)  
```csharp
public interface IPedidoRepository
{
    Task<Pedido?> ObterPorIdAsync(Guid id);
    Task SalvarAsync(Pedido pedido);
}
```

### 1.4.4 Exemplo de Adapter (Infraestrutura)  
```csharp
public class PedidoRepository : IPedidoRepository
{
    private readonly DbContext _context;

    public PedidoRepository(DbContext context)
    {
        _context = context;
    }

    public async Task<Pedido?> ObterPorIdAsync(Guid id)
    {
        return await _context.Set<Pedido>().FindAsync(id);
    }

    public async Task SalvarAsync(Pedido pedido)
    {
        _context.Update(pedido);
        await _context.SaveChangesAsync();
    }
}
```

### 1.4.5 Exemplo de Command (CQRS)  
```csharp
public record CriarPedidoCommand(Guid PedidoId) : IRequest;
```

### 1.4.6 Exemplo de Handler (CQRS + DDD)  
```csharp
public class CriarPedidoHandler : IRequestHandler<CriarPedidoCommand>
{
    private readonly IPedidoRepository _repository;

    public CriarPedidoHandler(IPedidoRepository repository)
    {
        _repository = repository;
    }

    public async Task Handle(CriarPedidoCommand request, CancellationToken cancellationToken)
    {
        var pedido = new Pedido(request.PedidoId);
        await _repository.SalvarAsync(pedido);
    }
}
```

### 1.4.7 Exemplo de Teste (TDD)  
```csharp
public class PedidoTests
{
    [Fact]
    public void DeveAdicionarItemComSucesso()
    {
        var pedido = new Pedido(Guid.NewGuid());
        pedido.AdicionarItem("Produto A", 2);

        Assert.Single(pedido.Itens);
    }
}
```

## 1.5 Exemplos Práticos  
### 1.5.1 Fluxo de Criação de Pedido  
1. API recebe o comando CriarPedidoCommand.  
2. Handler executa o caso de uso.  
3. Domínio cria a entidade Pedido.  
4. Repositório persiste o agregado.  
5. Evento de domínio pode ser publicado para outros microserviços.

### 1.5.2 Comparação: Arquitetura Tradicional vs Hexagonal  
| Aspecto | Tradicional | Hexagonal |
|--------|-------------|-----------|
| Acoplamento | Alto | Baixo |
| Testabilidade | Limitada | Elevada |
| Evolução | Difícil | Flexível |
| Dependência de Frameworks | Forte | Mínima |

## 1.6 Boas Práticas  
### 1.6.1 Microserviços  
- Evitar compartilhamento de banco de dados.  
- Garantir autonomia e independência de deploy.  
- Utilizar contratos bem definidos.

### 1.6.2 Hexagonal Architecture  
- Manter o domínio completamente isolado.  
- Criar ports claros e estáveis.  
- Adaptadores devem ser substituíveis sem impacto no domínio.

### 1.6.3 DDD  
- Focar no Ubiquitous Language.  
- Definir corretamente os Bounded Contexts.  
- Evitar anêmia de domínio.

### 1.6.4 TDD  
- Escrever testes antes da implementação.  
- Garantir cobertura de regras de negócio.  
- Refatorar continuamente.

### 1.6.5 CQRS  
- Separar modelos de leitura e escrita.  
- Utilizar eventos para sincronização quando necessário.  
- Evitar complexidade desnecessária em domínios simples.

### 1.6.6 SOLID  
- Aplicar princípios para manter coesão e baixo acoplamento.  
- Utilizar injeção de dependência.  
- Evitar interfaces excessivamente genéricas.

## 1.7 Conclusão  
A combinação de Microserviços, Hexagonal Architecture, DDD, TDD, CQRS e SOLID fornece uma base arquitetural sólida para sistemas distribuídos escaláveis, testáveis e alinhados ao domínio. Essa abordagem promove evolução contínua, qualidade de código e clareza na modelagem do negócio.

## 2 TDD

### 2.1 Introdução  
Test‑Driven Development (TDD) é uma prática de desenvolvimento que orienta a implementação de software por meio de testes automatizados escritos antes do código de produção. O objetivo é garantir qualidade, reduzir retrabalho, melhorar o design e promover segurança na evolução do sistema. Em arquiteturas modernas como Microserviços, Hexagonal Architecture, DDD, CQRS e SOLID, o TDD atua como um mecanismo essencial para manter o domínio isolado, testável e consistente.

### 2.2 Conceitos Fundamentais  
#### 2.2.1 Ciclo Red‑Green‑Refactor  
O TDD segue um ciclo contínuo composto por três etapas:  
- Red: escrever um teste que falha.  
- Green: implementar o mínimo necessário para o teste passar.  
- Refactor: melhorar o código mantendo todos os testes verdes.

#### 2.2.2 Papel do TDD em Arquiteturas Hexagonais  
O TDD reforça a separação entre domínio e infraestrutura, permitindo que o núcleo da aplicação seja testado sem dependências externas. Ports e Adapters facilitam a criação de testes isolados e previsíveis.

#### 2.2.3 TDD e DDD  
No contexto de DDD, o TDD garante que regras de negócio sejam validadas continuamente. Entidades, Value Objects e Domain Services tornam‑se altamente testáveis e livres de efeitos colaterais.

#### 2.2.4 TDD e CQRS  
Commands e Queries podem ser testados separadamente, garantindo que cada operação siga seu fluxo específico. Handlers tornam‑se unidades claras de teste.

#### 2.2.5 TDD e SOLID  
O TDD incentiva a aplicação dos princípios SOLID, especialmente Single Responsibility e Dependency Inversion, pois classes testáveis tendem a ser coesas e desacopladas.

### 2.3 Estrutura de Testes  
#### 2.3.1 Testes de Unidade  
Validam comportamentos isolados do domínio, sem dependências externas.

#### 2.3.2 Testes de Integração  
Validam a interação entre componentes, como repositórios e adaptadores.

#### 2.3.3 Testes de Contrato  
Essenciais em microserviços para garantir compatibilidade entre serviços consumidores e provedores.

#### 2.3.4 Testes End‑to‑End  
Simulam fluxos completos, garantindo que o sistema funcione como um todo.

### 2.4 Sintaxe e Exemplos de Código  
#### 2.4.1 Exemplo de Teste de Unidade  
```csharp
public class CalculadoraDescontosTests
{
    [Fact]
    public void DeveAplicarDescontoDe10PorCento()
    {
        var servico = new CalculadoraDescontos();
        var resultado = servico.Calcular(100m);

        Assert.Equal(90m, resultado);
    }
}
```

#### 2.4.2 Implementação Mínima (Green)  
```csharp
public class CalculadoraDescontos
{
    public decimal Calcular(decimal valor)
    {
        return valor * 0.9m;
    }
}
```

#### 2.4.3 Refatoração  
```csharp
public class CalculadoraDescontos
{
    private const decimal FatorDesconto = 0.9m;

    public decimal Calcular(decimal valor)
    {
        return valor * FatorDesconto;
    }
}
```

#### 2.4.4 Testando Entidade de Domínio (DDD)  
```csharp
public class PedidoTests
{
    [Fact]
    public void DeveAdicionarItemAoPedido()
    {
        var pedido = new Pedido(Guid.NewGuid());
        pedido.AdicionarItem("Produto X", 3);

        Assert.Single(pedido.Itens);
    }
}
```

#### 2.4.5 Testando Handler CQRS  
```csharp
public class CriarPedidoHandlerTests
{
    [Fact]
    public async Task DeveCriarPedidoComSucesso()
    {
        var repo = Substitute.For<IPedidoRepository>();
        var handler = new CriarPedidoHandler(repo);

        var command = new CriarPedidoCommand(Guid.NewGuid());
        await handler.Handle(command, CancellationToken.None);

        await repo.Received(1).SalvarAsync(Arg.Any<Pedido>());
    }
}
```

### 2.5 Exemplos Práticos  
#### 2.5.1 Fluxo Completo com TDD  
1. Criar teste para validar criação de pedido.  
2. Implementar entidade Pedido com o mínimo necessário.  
3. Criar teste para adicionar itens.  
4. Implementar lógica de itens.  
5. Criar teste para persistência via handler.  
6. Implementar handler e repositório fake.  
7. Refatorar mantendo todos os testes verdes.

#### 2.5.2 Comparação: Desenvolvimento Tradicional vs TDD  
| Aspecto | Tradicional | TDD |
|--------|-------------|-----|
| Ordem | Código → Teste | Teste → Código |
| Qualidade | Variável | Alta e consistente |
| Refatoração | Arriscada | Segura |
| Cobertura | Baixa | Elevada |
| Design | Reativo | Intencional |

### 2.6 Boas Práticas  
#### 2.6.1 Escrever Testes Claros  
- Nomear testes de forma descritiva.  
- Testar apenas um comportamento por teste.

#### 2.6.2 Evitar Dependências Externas  
- Utilizar mocks, stubs e fakes.  
- Não acessar banco de dados em testes de unidade.

#### 2.6.3 Manter Testes Determinísticos  
- Evitar uso de datas e valores aleatórios sem controle.  
- Garantir que testes sempre produzam o mesmo resultado.

#### 2.6.4 Cobrir Regras de Negócio  
- Priorizar testes do domínio.  
- Validar invariantes e comportamentos críticos.

#### 2.6.5 Refatorar com Segurança  
- Executar testes continuamente.  
- Garantir que o domínio permaneça coeso e desacoplado.

### 2.7 Conclusão  
O TDD é uma prática essencial para garantir qualidade, previsibilidade e segurança no desenvolvimento de sistemas complexos. Em conjunto com Microserviços, Hexagonal Architecture, DDD, CQRS e SOLID, o TDD fortalece o design, reduz acoplamento e promove evolução sustentável do software.

# 3 Preparação do Ambiente
## 3.1 Organizando a Estrutura do Projeto

### 3.1.1 Introdução  
A organização da solução é um dos pilares fundamentais para garantir clareza arquitetural, isolamento de responsabilidades e facilidade de manutenção. Em um ecossistema baseado em Microserviços, Hexagonal Architecture, DDD, TDD, CQRS e SOLID, a estrutura física do projeto deve refletir a separação lógica entre camadas, contextos e responsabilidades.

A seguir, apresenta‑se a estrutura proposta para o serviço **BookingService**, incluindo Adapters, Core (Application e Domain) e Tests, além da preparação para o **PaymentService**.

### 3.1.2 Estrutura Geral da Solução  
A solução é organizada em pastas que representam camadas e contextos. Cada projeto é isolado fisicamente e semanticamente, permitindo evolução independente e testes específicos.

```xml
<Solution>
  <Folder Name="/BookingService/" />
  <Folder Name="/BookingService/Adapters/">
    <Project Path="BookingService/Adapters/Data/Data.csproj" Id="b0d3845b-3a0d-4666-aab6-beb8de6e693e" />
  </Folder>
  <Folder Name="/BookingService/Consumers/">
    <Project Path="BookingService/Consumers/API/API.csproj" Id="153d2363-370f-4011-b98f-3da21c1c397c" />
  </Folder>
  <Folder Name="/BookingService/Core/" />
  <Folder Name="/BookingService/Core/Application/">
    <Project Path="BookingService/Core/Application/Application/Application.csproj" />
  </Folder>
  <Folder Name="/BookingService/Core/Domain/">
    <Project Path="BookingService/Core/Domain/Domain/Domain.csproj" />
  </Folder>
  <Folder Name="/BookingService/Tests/" />
  <Folder Name="/BookingService/Tests/Adapters/">
    <Project Path="BookingService/Tests/Adapters/AdaptersTests/AdaptersTests.csproj" Id="e3f91762-17e1-4a85-8456-c5720fd07909" />
  </Folder>
  <Folder Name="/BookingService/Tests/Application/">
    <Project Path="BookingService/Tests/Application/ApplicationTests/ApplicationTests.csproj" Id="3704e687-007d-4644-b1cc-ab6cf78ca40d" />
  </Folder>
  <Folder Name="/BookingService/Tests/Domain/">
    <Project Path="BookingService/Tests/Domain/DomainTests/DomainTests.csproj" />
  </Folder>
  <Folder Name="/PaymentService/" />
</Solution>
```

### 3.1.3 Interpretação da Estrutura  
#### 3.1.3.1 BookingService  
O serviço de reservas é dividido em três grandes áreas:

- **Adapters**  
  Contém implementações externas, como persistência, mensageria, integrações e APIs.  
  Exemplo: `Data.csproj` representa o adapter de acesso a dados.

- **Consumers**  
  Contém interfaces de entrada, como APIs REST, gRPC ou mensageria.  
  Exemplo: `API.csproj` representa o ponto de entrada do serviço.

- **Core**  
  Contém o núcleo da aplicação dividido em:
  - **Application**: casos de uso, comandos, queries, orquestração.  
  - **Domain**: entidades, agregados, value objects, regras de negócio.

- **Tests**  
  Cada camada possui sua própria suíte de testes:
  - Tests/Adapters  
  - Tests/Application  
  - Tests/Domain  

Essa separação permite aplicar TDD de forma granular e alinhada ao DDD.

#### 3.1.3.2 PaymentService  
A pasta está preparada para receber a mesma estrutura modular, garantindo consistência entre microserviços.

### 3.1.4 Exemplo de Estrutura Hexagonal Aplicada  
A estrutura reflete diretamente os princípios da Arquitetura Hexagonal:

| Camada | Localização | Responsabilidade |
|--------|-------------|------------------|
| Domínio | Core/Domain | Regras de negócio puras |
| Aplicação | Core/Application | Casos de uso, orquestração |
| Adapters | Adapters | Implementações externas |
| Entradas | Consumers | APIs, mensageria |
| Testes | Tests | Validação isolada por camada |

### 3.1.5 Exemplo de Código: Port e Adapter  
#### 3.1.5.1 Port no Domínio  
```csharp
namespace BookingService.Core.Domain.Ports
{
    public interface IBookingRepository
    {
        Task<Booking?> ObterPorIdAsync(Guid id);
        Task SalvarAsync(Booking booking);
    }
}
```

#### 3.1.5.2 Adapter de Persistência  
```csharp
namespace BookingService.Adapters.Data
{
    public class BookingRepository : IBookingRepository
    {
        private readonly BookingDbContext _context;

        public BookingRepository(BookingDbContext context)
        {
            _context = context;
        }

        public async Task<Booking?> ObterPorIdAsync(Guid id)
        {
            return await _context.Bookings.FindAsync(id);
        }

        public async Task SalvarAsync(Booking booking)
        {
            _context.Update(booking);
            await _context.SaveChangesAsync();
        }
    }
}
```

### 3.1.6 Boas Práticas de Organização  
#### 3.1.6.1 Separação Estrita por Responsabilidade  
Cada projeto deve conter apenas elementos da sua camada.  
Exemplo: o domínio nunca referencia adapters.

#### 3.1.6.2 Independência de Build  
Cada projeto deve compilar isoladamente, reforçando o desacoplamento.

#### 3.1.6.3 Testes por Camada  
A divisão de testes por camada permite validar comportamentos específicos sem interferência externa.

#### 3.1.6.4 Nomeação Consistente  
Manter padrões como:  
- `<Context>.Domain`  
- `<Context>.Application`  
- `<Context>.Adapters`  
- `<Context>.Tests`

### 3.1.7 Conclusão  
A estrutura apresentada garante alinhamento com princípios de modularidade, isolamento e testabilidade. Ela serve como base sólida para evolução contínua, permitindo que cada microserviço cresça de forma organizada, previsível e aderente aos padrões arquiteturais modernos.

# 4 Criando As Camandas

## 4.1 Estruturando a Camada de Domínio

### 4.1.1 Introdução  
A camada de domínio é o núcleo da aplicação e concentra as regras de negócio, invariantes, entidades, objetos de valor, eventos e contratos (ports). Em uma arquitetura orientada por DDD e Hexagonal Architecture, o domínio deve permanecer completamente isolado de frameworks, infraestrutura e detalhes externos. O código apresentado demonstra uma estrutura inicial de entidades, enumerações e interfaces, que será refinada para refletir corretamente os princípios de modelagem do domínio.

### 4.1.2 Entidades do Domínio  
As entidades representam conceitos centrais do negócio que possuem identidade e ciclo de vida próprio. A seguir, cada entidade é analisada e ajustada conforme boas práticas de DDD.

#### 4.1.2.1 Entidade Booking  
```csharp
using Domain.Enums;

namespace Domain.Entities;

public class Booking
{
    public int Id { get; private set; }
    public DateTime PlacedAt { get; private set; }
    public DateTime Start { get; private set; }
    public DateTime End { get; private set; }
    private Status Status { get; set; }

    public Status CurrentStatus => Status;

    public Booking(int id, DateTime start, DateTime end)
    {
        if (end <= start)
            throw new ArgumentException("End date must be greater than start date.");

        Id = id;
        PlacedAt = DateTime.UtcNow;
        Start = start;
        End = end;
        Status = Status.Created;
    }

    public void MarkAsPaid()
    {
        if (Status != Status.Created)
            throw new InvalidOperationException("Only newly created bookings can be paid.");

        Status = Status.Paid;
    }

    public void Cancel()
    {
        if (Status == Status.Finished)
            throw new InvalidOperationException("Finished bookings cannot be canceled.");

        Status = Status.Canceled;
    }
}
```

**Características importantes:**  
- Construtor garante invariantes.  
- Status é controlado internamente.  
- Métodos de domínio representam comportamentos, não simples mutações.

#### 4.1.2.2 Entidade Guest  
```csharp
namespace Domain.Entities;

public class Guest
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public string Surname { get; private set; }
    public string Email { get; private set; }

    public Guest(int id, string name, string surname, string email)
    {
        Id = id;
        Name = name;
        Surname = surname;
        Email = email;
    }
}
```

**Pontos relevantes:**  
- Propriedades tornam‑se imutáveis após construção.  
- Entidade simples, mas essencial para o agregado Booking.

#### 4.1.2.3 Entidade Room  
```csharp
namespace Domain.Entities;

public class Room
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public int Level { get; private set; }
    public bool InMaintenance { get; private set; }

    public bool IsAvailable => !InMaintenance && !HasGuest;

    public bool HasGuest => false; // Placeholder até integração com Booking

    public Room(int id, string name, int level, bool inMaintenance)
    {
        Id = id;
        Name = name;
        Level = level;
        InMaintenance = inMaintenance;
    }
}
```

**Observações:**  
- `HasGuest` deve futuramente ser integrado ao agregado Booking.  
- `IsAvailable` representa uma regra de negócio clara.

### 4.1.3 Enumerações  
Enumerações representam estados possíveis dentro do domínio.

```csharp
namespace Domain.Enums;

public enum Status
{
    Created,
    Paid,
    Finished,
    Canceled,
    Refounded
}
```

### 4.1.4 Ports (Interfaces do Domínio)  
Ports representam contratos que a camada de aplicação utilizará para interagir com o domínio. Eles são implementados por adapters na infraestrutura.

```csharp
using Domain.Entities;

namespace Domain.Ports;

public interface IGuestRepository
{
    Task<Guest> Get(int id);
    Task<int> Save(Guest guest);
}
```

**Características:**  
- Não há dependência de frameworks.  
- Define operações essenciais para persistência de Guest.  
- Mantém o domínio isolado de detalhes técnicos.

### 4.1.5 Boas Práticas na Estruturação do Domínio  
#### 4.1.5.1 Imutabilidade Sempre que Possível  
Entidades devem expor apenas comportamentos, não setters públicos.

#### 4.1.5.2 Regras de Negócio no Domínio  
Validações e transições de estado devem ocorrer dentro das entidades.

#### 4.1.5.3 Domínio Livre de Frameworks  
Nada de EF Core, bibliotecas de mensageria ou dependências externas.

#### 4.1.5.4 Ports como Contratos Estáveis  
Interfaces devem ser pequenas, claras e orientadas ao domínio.

### 4.1.6 Exemplo Prático de Uso  
```csharp
var booking = new Booking(1, DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(3));

booking.MarkAsPaid();

if (booking.CurrentStatus == Status.Paid)
{
    // Fluxo de confirmação de pagamento
}
```

### 4.1.7 Conclusão  
A camada de domínio deve ser o centro da modelagem, refletindo regras, comportamentos e invariantes do negócio. A estrutura apresentada segue os princípios de DDD e Hexagonal Architecture, garantindo isolamento, clareza e testabilidade. Essa base sólida permite que camadas superiores evoluam sem comprometer a integridade do domínio.

## 4.2 Criando uma Máquina de Estado para as Reservas

### 4.2.1 Introdução  
A modelagem de uma máquina de estados dentro do domínio permite controlar transições válidas entre diferentes fases do ciclo de vida de uma reserva. Em DDD, esse mecanismo deve estar encapsulado na própria entidade, garantindo que regras de negócio sejam respeitadas e que estados inválidos não possam ser alcançados.

A abordagem apresentada utiliza *pattern matching* para definir transições de forma clara, expressiva e totalmente alinhada ao domínio.

### 4.2.2 Enumerações de Ações  
A enumeração `Action` representa eventos que podem ocorrer sobre uma reserva. Cada ação corresponde a uma possível transição de estado.

```csharp
namespace Domain.Enums;

public enum Action
{
    Pay,
    Finish,   // after paid and used
    Cancel,   // can never be paid
    Refound,  // paid then refunded
    Reopen    // canceled
}
```

### 4.2.3 Máquina de Estado na Entidade Booking  
A entidade `Booking` passa a controlar suas transições internas por meio do método `ChangeState`. A lógica utiliza *switch expressions* para mapear combinações de estado atual + ação desejada.

```csharp
using Domain.Enums;
using Action = Domain.Enums.Action;

namespace Domain.Entities;

public class Booking
{
    public int Id { get; private set; }
    public DateTime PlacedAt { get; private set; }
    public DateTime Start { get; private set; }
    public DateTime End { get; private set; }
    private Status Status { get; set; }

    public Status CurrentStatus => Status;

    public Booking(int id, DateTime start, DateTime end)
    {
        if (end <= start)
            throw new ArgumentException("End date must be greater than start date.");

        Id = id;
        PlacedAt = DateTime.UtcNow;
        Start = start;
        End = end;
        Status = Status.Created;
    }

    public void ChangeState(Action action)
    {
        Status = (Status, action) switch
        {
            (Status.Created,  Action.Pay)     => Status.Paid,
            (Status.Created,  Action.Cancel)  => Status.Canceled,
            (Status.Paid,     Action.Finish)  => Status.Finished,
            (Status.Paid,     Action.Refound) => Status.Refounded,
            (Status.Canceled, Action.Reopen)  => Status.Created,
            _ => Status
        };
    }
}
```

### 4.2.4 Análise das Transições  
A tabela abaixo resume as transições válidas:

| Estado Atual | Ação | Próximo Estado |
|--------------|-------|----------------|
| Created | Pay | Paid |
| Created | Cancel | Canceled |
| Paid | Finish | Finished |
| Paid | Refound | Refounded |
| Canceled | Reopen | Created |

Transições não previstas são ignoradas, mantendo o estado atual. Isso evita estados inválidos sem necessidade de exceções.

### 4.2.5 Boas Práticas na Modelagem da Máquina de Estado  
#### 4.2.5.1 Estados e Ações Claramente Definidos  
Cada estado deve representar uma fase real do processo de reserva.

#### 4.2.5.2 Transições Determinísticas  
A combinação estado + ação deve sempre resultar em um estado previsível.

#### 4.2.5.3 Encapsulamento  
A entidade controla suas próprias transições, evitando que camadas externas manipulem estados diretamente.

#### 4.2.5.4 Extensibilidade  
Novos estados ou ações podem ser adicionados sem alterar o comportamento existente.

### 4.2.6 Exemplo Prático  
```csharp
var booking = new Booking(1, DateTime.UtcNow.AddDays(1), DateTime.UtcNow.AddDays(3));

booking.ChangeState(Action.Pay);
booking.ChangeState(Action.Finish);

if (booking.CurrentStatus == Status.Finished)
{
    // Fluxo de pós-utilização da reserva
}
```

### 4.2.7 Conclusão  
A máquina de estados implementada na entidade `Booking` garante consistência, previsibilidade e aderência às regras de negócio. Essa abordagem fortalece o domínio, reduz erros e facilita a evolução do sistema, mantendo a lógica central encapsulada e expressiva.

## 4.3 Escrevendo Unit Tests para a Máquina de Estado

### 4.3.1 Introdução  
A escrita de testes unitários para a máquina de estado garante que todas as transições válidas sejam respeitadas e que transições inválidas não alterem o estado da reserva. Essa abordagem segue TDD e reforça a robustez do domínio, assegurando que a entidade `Booking` mantenha comportamento previsível ao longo de sua evolução.

Os testes apresentados utilizam NUnit e cobrem cenários positivos (transições válidas) e negativos (transições inválidas).

### 4.3.2 Ajustando o Construtor da Entidade  
Os testes utilizam `new Booking()` sem parâmetros. Para compatibilidade, a entidade deve possuir um construtor padrão que inicialize o estado como `Created`.

```csharp
public Booking()
{
    PlacedAt = DateTime.UtcNow;
    Status = Status.Created;
}
```

### 4.3.3 Testes Unitários para Transições Válidas  
Os testes abaixo validam que a máquina de estado responde corretamente às ações permitidas.

```csharp
using Domain.Entities;
using Domain.Enums;
using Action = Domain.Enums.Action;

namespace DomainTests.Bookings;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void ShouldAlwaysStartWithCreatedStatus()
    {
        var booking = new Booking();
        Assert.That(booking.CurrentStatus, Is.EqualTo(Status.Created));
    }

    [Test]
    public void ShouldSetStatusPaidWhenPayingForABookingWithCreatedStatus()
    {
        var booking = new Booking();
        booking.ChangeState(Action.Pay);
        Assert.That(booking.CurrentStatus, Is.EqualTo(Status.Paid));
    }

    [Test]
    public void ShouldSetStatusCanceledWhenCancelingForABookingWithCreatedStatus()
    {
        var booking = new Booking();
        booking.ChangeState(Action.Cancel);
        Assert.That(booking.CurrentStatus, Is.EqualTo(Status.Canceled));
    }

    [Test]
    public void ShouldSetStatusFinishedWhenFinishingForABookingWithPaidStatus()
    {
        var booking = new Booking();
        booking.ChangeState(Action.Pay);
        booking.ChangeState(Action.Finish);
        Assert.That(booking.CurrentStatus, Is.EqualTo(Status.Finished));
    }

    [Test]
    public void ShouldSetStatusRefoundedWhenRefoundingForABookingWithPaidStatus()
    {
        var booking = new Booking();
        booking.ChangeState(Action.Pay);
        booking.ChangeState(Action.Refound);
        Assert.That(booking.CurrentStatus, Is.EqualTo(Status.Refounded));
    }

    [Test]
    public void ShouldSetStatusCreatedWhenReopeningForABookingWithCanceledStatus()
    {
        var booking = new Booking();
        booking.ChangeState(Action.Cancel);
        booking.ChangeState(Action.Reopen);
        Assert.That(booking.CurrentStatus, Is.EqualTo(Status.Created));
    }
}
```

### 4.3.4 Testes Unitários para Transições Inválidas  
Os testes seguintes garantem que ações não permitidas não alterem o estado atual.

```csharp
[Test]
public void ShouldNotChangeWhenRefoundingForABookingWithCreatedStatus()
{
    var booking = new Booking();
    booking.ChangeState(Action.Refound);
    Assert.That(booking.CurrentStatus, Is.EqualTo(Status.Created));
}

[Test]
public void ShouldNotChangeWhenRefoundingForABookingWithFinishedStatus()
{
    var booking = new Booking();
    booking.ChangeState(Action.Pay);
    booking.ChangeState(Action.Finish);
    booking.ChangeState(Action.Refound);
    Assert.That(booking.CurrentStatus, Is.EqualTo(Status.Finished));
}

[Test]
public void ShouldNotChangeWhenFinishingForABookingWithCreatedStatus()
{
    var booking = new Booking();
    booking.ChangeState(Action.Finish);
    Assert.That(booking.CurrentStatus, Is.EqualTo(Status.Created));
}

[Test]
public void ShouldNotChangeWhenReopeningForABookingWithCreatedStatus()
{
    var booking = new Booking();
    booking.ChangeState(Action.Reopen);
    Assert.That(booking.CurrentStatus, Is.EqualTo(Status.Created));
}

[Test]
public void ShouldNotChangeWhenCancelingForABookingWithPaidStatus()
{
    var booking = new Booking();
    booking.ChangeState(Action.Pay);
    booking.ChangeState(Action.Cancel);
    Assert.That(booking.CurrentStatus, Is.EqualTo(Status.Paid));
}
```

### 4.3.5 Boas Práticas na Escrita dos Testes  
#### 4.3.5.1 Testes Devem Ser Determinísticos  
Nenhum teste deve depender de valores variáveis ou externos.

#### 4.3.5.2 Um Cenário por Teste  
Cada teste valida apenas uma regra da máquina de estado.

#### 4.3.5.3 Nomeação Descritiva  
Os nomes dos testes descrevem claramente o comportamento esperado.

#### 4.3.5.4 Cobertura Completa  
Todos os caminhos válidos e inválidos devem ser testados para garantir previsibilidade.

### 4.3.6 Conclusão  
Os testes unitários apresentados validam integralmente a máquina de estado da entidade `Booking`, garantindo que transições válidas sejam aplicadas corretamente e que transições inválidas não modifiquem o estado. Essa abordagem fortalece o domínio, assegura consistência e permite evolução segura da lógica de negócio.

## 4.4 Conectando com Banco de Dados e Habilitando as Migrations

### 4.4.1 Introdução  
A integração entre a camada de infraestrutura e o banco de dados é um passo essencial para persistir entidades do domínio. Em uma arquitetura Hexagonal, essa integração ocorre exclusivamente nos *adapters*, mantendo o domínio isolado. Nesta seção, é apresentada a configuração do Entity Framework Core, a criação do `DbContext`, o registro no container de injeção de dependências e a execução das migrations.

### 4.4.2 Estrutura Recomendada  
A camada **Adapters/Data** é responsável por conter:

- O `DbContext`
- As configurações de mapeamento (quando aplicável)
- As migrations
- Implementações de repositórios

Essa separação garante que o domínio e a aplicação permaneçam independentes de detalhes de persistência.

### 4.4.3 Criando e Aplicando Migrations  
As migrations devem ser geradas no projeto de infraestrutura (`Adapters/Data`), mas executadas a partir do projeto de API (`Consumers/API`), que contém o *startup* da aplicação.

Comandos:

```
dotnet ef migrations add InitialCreation -s .\BookingService\Consumers\API -p .\BookingService\Adapters\Data
dotnet ef database update -s .\BookingService\Consumers\API
```

**Explicação dos parâmetros:**

| Parâmetro | Significado |
|----------|-------------|
| `-s` | Projeto que contém o *startup* (API) |
| `-p` | Projeto onde o `DbContext` está localizado (Data) |

### 4.4.4 Configurando o DbContext na API  
A API deve registrar o contexto no container de DI, utilizando a connection string definida no `appsettings.json`.

```csharp
using Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

#region Database Context
var connectionString = builder.Configuration.GetConnectionString("Main");

builder.Services.AddDbContext<HotelDBContext>(options =>
    options.UseNpgsql(connectionString)
);
#endregion

#region OpenAPI Documentation
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#endregion

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
```

### 4.4.5 Implementando o DbContext  
O `DbContext` deve refletir apenas as entidades que pertencem ao agregado persistido. No exemplo, apenas `Guest` está mapeado.

```csharp
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class HotelDBContext : DbContext
    {
        public HotelDBContext(DbContextOptions<HotelDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Guest> Guests { get; set; }
    }
}
```

### 4.4.6 Boas Práticas na Configuração de Persistência  
#### 4.4.6.1 Domínio Não Deve Referenciar EF Core  
O domínio permanece puro, sem atributos ou dependências de persistência.

#### 4.4.6.2 Migrations Sempre no Projeto de Infraestrutura  
Isso evita acoplamento entre API e detalhes de banco.

#### 4.4.6.3 Connection Strings Apenas na API  
A API é responsável por orquestrar infraestrutura, não o domínio.

#### 4.4.6.4 DbContext Segue o Bounded Context  
Cada microserviço possui seu próprio banco e seu próprio contexto.

### 4.4.7 Exemplo Prático de Execução  
1. Criar o `DbContext` no projeto `Adapters/Data`.  
2. Registrar o contexto no `Program.cs` da API.  
3. Executar o comando de migration.  
4. Atualizar o banco com `database update`.  
5. Verificar se a tabela `Guests` foi criada corretamente.

### 4.4.8 Conclusão  
A configuração apresentada estabelece a base para persistência no microserviço, mantendo o isolamento arquitetural e permitindo evolução segura. A separação entre API, domínio e infraestrutura garante aderência aos princípios de DDD e Hexagonal Architecture, enquanto o uso do Entity Framework Core facilita a criação e manutenção do banco de dados.

## 4.5 Criando as Tabelas Faltantes

### 4.5.1 Introdução  
Após configurar o contexto de banco de dados e habilitar as migrations, o próximo passo é incluir as entidades restantes no `DbContext` para que o Entity Framework Core gere as tabelas correspondentes. Nesta etapa, são adicionadas as entidades `Room` e `Booking`, permitindo que o microserviço persista reservas, hóspedes e quartos.

### 4.5.2 Atualizando o DbContext  
A camada **Adapters/Data** deve refletir todas as entidades que pertencem ao *bounded context* de Booking. O `HotelDBContext` é atualizado para incluir `Rooms` e `Bookings`.

```csharp
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class HotelDBContext : DbContext
    {
        public HotelDBContext(DbContextOptions<HotelDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Guest> Guests { get; set; }
        public virtual DbSet<Room> Rooms { get; set; }
        public virtual DbSet<Booking> Bookings { get; set; }
    }
}
```

### 4.5.3 Atualizando a Entidade Booking  
A entidade `Booking` agora possui relacionamentos com `Room` e `Guest`. Esses relacionamentos serão refletidos automaticamente nas migrations.

```csharp
using Domain.Enums;
using Action = Domain.Enums.Action;

namespace Domain.Entities;

public class Booking
{
    public int Id { get; set; }
    public DateTime PlacedAt { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    private Status Status { get; set; }

    public Room Room { get; set; } = null!;
    public Guest Guest { get; set; } = null!;

    public Status CurrentStatus => Status;

    public Booking()
    {
        Status = Status.Created;
    }

    public void ChangeState(Action action)
    {
        Status = (Status, action) switch
        {
            (Status.Created,    Action.Pay)     => Status.Paid,
            (Status.Created,    Action.Cancel)  => Status.Canceled,
            (Status.Paid,       Action.Finish)  => Status.Finished,
            (Status.Paid,       Action.Refound) => Status.Refounded,
            (Status.Canceled,   Action.Reopen)  => Status.Created,
            _ => Status
        };
    }
}
```

### 4.5.4 Gerando a Migration  
Com o `DbContext` atualizado, a migration pode ser criada:

```
dotnet ef migrations add RoomsAndBookings -s .\BookingService\Consumers\API -p .\BookingService\Adapters\Data
```

### 4.5.5 Atualizando o Banco de Dados  
Após gerar a migration, o banco é atualizado:

```
dotnet ef database update -s .\BookingService\Consumers\API
```

### 4.5.6 Estrutura Esperada das Tabelas  
Após a execução da migration, o banco deve conter:

| Tabela | Descrição |
|--------|-----------|
| Guests | Armazena hóspedes |
| Rooms | Armazena quartos |
| Bookings | Armazena reservas, incluindo chaves estrangeiras para Guest e Room |

### 4.5.7 Boas Práticas  
#### 4.5.7.1 Relacionamentos Claros  
As entidades devem refletir relações reais do domínio (ex.: uma reserva sempre possui um hóspede e um quarto).

#### 4.5.7.2 Migrations Versionadas  
Cada alteração no domínio persistido deve gerar uma nova migration, mantendo histórico de evolução.

#### 4.5.7.3 Contexto Enxuto  
Somente entidades pertencentes ao *bounded context* devem ser incluídas no `DbContext`.

### 4.5.8 Conclusão  
Com a inclusão das entidades `Room` e `Booking` no `DbContext` e a execução das migrations, o microserviço passa a ter suporte completo para persistência de reservas, hóspedes e quartos. Essa etapa consolida a integração entre domínio e infraestrutura, mantendo a arquitetura organizada e alinhada aos princípios de DDD e Hexagonal Architecture.

## 4.6 Utilizando Value Objects

### 4.6.1 Introdução  
Value Objects representam conceitos do domínio que **não possuem identidade própria**, sendo definidos exclusivamente por seus atributos. Eles reforçam invariantes, encapsulam regras de negócio e aumentam a expressividade do modelo. Em DDD, Value Objects são imutáveis e devem ser tratados como unidades atômicas de significado.

Nesta seção, são apresentados dois Value Objects aplicados ao domínio de reservas: **PersonId** (documento de identificação do hóspede) e **Price** (valor e moeda do quarto). Também é demonstrado como configurá‑los corretamente no Entity Framework Core utilizando *owned types*.

---

### 4.6.2 Criando o Value Object PersonId  
O Value Object `PersonId` representa um documento de identificação, composto por um número e um tipo de documento.

```csharp
using Domain.Enums;

namespace Domain.ValueObjects;

public class PersonId
{
    public string IdNumber { get; private set; } = null!;
    public DocumentTypes DocumentType { get; private set; }

    public PersonId(string idNumber, DocumentTypes documentType)
    {
        IdNumber = idNumber;
        DocumentType = documentType;
    }
}
```

#### Características importantes  
- Imutável após construção.  
- Representa um conceito único do domínio.  
- Não possui identidade própria.  

---

### 4.6.3 Enumeração DocumentTypes  
```csharp
namespace Domain.Enums;

public enum DocumentTypes
{
    Passaporte,
    DriverLicense
}
```

---

### 4.6.4 Aplicando PersonId na Entidade Guest  
A entidade `Guest` passa a utilizar o Value Object como parte de sua composição.

```csharp
using Domain.ValueObjects;

namespace Domain.Entities
{
    public class Guest
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Surname { get; set; }
        public required string Email { get; set; }

        public PersonId DocumentId { get; set; } = null!;
    }
}
```

---

### 4.6.5 Configurando PersonId no Entity Framework Core  
A configuração utiliza *owned types*, garantindo que o Value Object seja persistido na mesma tabela do agregado.

```csharp
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data;

public class GuestConfiguration : IEntityTypeConfiguration<Guest>
{
    public void Configure(EntityTypeBuilder<Guest> builder)
    {
        builder.HasKey(x => x.Id);

        builder.OwnsOne(x => x.DocumentId)
            .Property(x => x.IdNumber);

        builder.OwnsOne(x => x.DocumentId)
            .Property(x => x.DocumentType);
    }
}
```

---

### 4.6.6 Criando o Value Object Price  
O Value Object `Price` representa o valor monetário de um quarto.

```csharp
using Domain.Enums;

namespace Domain.ValueObjects;

public class Price
{
    public decimal Value { get; private set; }
    public AcceptedCurrencies Currency { get; private set; }

    public Price(decimal value, AcceptedCurrencies currency)
    {
        Value = value;
        Currency = currency;
    }
}
```

---

### 4.6.7 Enumeração AcceptedCurrencies  
```csharp
namespace Domain.Enums;

public enum AcceptedCurrencies
{
    Dollar,
    Euro,
    Bitcoin
}
```

---

### 4.6.8 Aplicando Price na Entidade Room  
```csharp
using Domain.ValueObjects;

namespace Domain.Entities;

public class Room
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int Level { get; set; }
    public bool InMaintenance { get; set; }

    public Price Price { get; set; } = null!;

    public bool IsAvailable =>
        !(InMaintenance || HasGuest);

    public bool HasGuest => true;
}
```

---

### 4.6.9 Configurando Price no Entity Framework Core  
```csharp
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Data;

public class RoomConfiguration : IEntityTypeConfiguration<Room>
{
    public void Configure(EntityTypeBuilder<Room> builder)
    {
        builder.HasKey(x => x.Id);

        builder.OwnsOne(x => x.Price)
            .Property(x => x.Value);

        builder.OwnsOne(x => x.Price)
            .Property(x => x.Currency);
    }
}
```

---

### 4.6.10 Atualizando o DbContext  
O contexto aplica as configurações automaticamente.

```csharp
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class HotelDBContext : DbContext
    {
        public HotelDBContext(DbContextOptions<HotelDBContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Guest> Guests { get; set; }
        public virtual DbSet<Room> Rooms { get; set; }
        public virtual DbSet<Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new GuestConfiguration());
            modelBuilder.ApplyConfiguration(new RoomConfiguration());
        }
    }
}
```

---

### 4.6.11 Boas Práticas ao Utilizar Value Objects  
#### 4.6.11.1 Imutabilidade  
Value Objects devem ser criados completos e nunca modificados.

#### 4.6.11.2 Sem Identidade  
Eles não possuem chave primária própria.

#### 4.6.11.3 Representam Conceitos do Domínio  
Documentos, preços, endereços, períodos, coordenadas, etc.

#### 4.6.11.4 Persistência com Owned Types  
Mantém o domínio limpo e o mapeamento transparente.

---

### 4.6.12 Conclusão  
A introdução de Value Objects como `PersonId` e `Price` aumenta a expressividade do domínio, reforça invariantes e reduz inconsistências. A combinação com *owned types* do Entity Framework Core garante persistência adequada sem comprometer a pureza do domínio. Essa abordagem fortalece o modelo e mantém a arquitetura alinhada aos princípios de DDD e Hexagonal Architecture.

# 5 Features De Serviço

## 5.1 Implementando o Use Case **CreateGuest**

### 5.1.1 Introdução  
O caso de uso **CreateGuest** representa a operação de criação de um hóspede dentro do *bounded context* de reservas. Ele pertence à camada **Application**, responsável por orquestrar regras de negócio, validar dados, interagir com repositórios (ports) e retornar respostas padronizadas.

A implementação segue os princípios de DDD, Hexagonal Architecture e SOLID, garantindo:

- Separação clara entre **DTOs**, **Requests**, **Responses**, **Use Cases** e **Repositories**.  
- Independência do domínio em relação à infraestrutura.  
- Testabilidade e previsibilidade do fluxo.

---

### 5.1.2 Estrutura do DTO  
O DTO é utilizado para transportar dados entre camadas externas (API) e a aplicação. Ele também contém o método de mapeamento para a entidade de domínio.

```csharp
using Domain.Enums;
using Domain.ValueObjects;
using Entities = Domain.Entities;

namespace Application.Guest.DTOs;

public class GuestDTO
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Surname { get; set; }
    public required string Email { get; set; }

    public string IdNumber { get; set; } = null!;
    public int IdTypeCode { get; set; }

    public static Entities.Guest MapToEntity(GuestDTO guestDTO)
    {
        return new Entities.Guest
        {
            Id = guestDTO.Id,
            Name = guestDTO.Name,
            Surname = guestDTO.Surname,
            Email = guestDTO.Email,
            DocumentId = new PersonId(
                guestDTO.IdNumber,
                (DocumentTypes)guestDTO.IdTypeCode
            )
        };
    }
}
```

---

### 5.1.3 Estrutura de Respostas da Aplicação  
A camada de aplicação utiliza respostas padronizadas para facilitar o consumo pela API.

```csharp
namespace Application;

public enum ErrorCodes
{
    NOT_FOUND = 1,
    COULD_NOT_STORE_DATA = 2
}

public abstract class Response
{
    public bool Sucess { get; set; }
    public string Message { get; set; } = null!;
    public ErrorCodes ErrorCode { get; set; }
}
```

#### Resposta específica para Guest

```csharp
using Application.Guest.DTOs;

namespace Application.Guest.Responses;

public class GuestResponse : Response
{
    public GuestDTO Data = null!;
}
```

---

### 5.1.4 Estrutura da Request  
A request encapsula os dados necessários para o caso de uso.

```csharp
using Application.Guest.DTOs;

namespace Application.Guest.Requests;

public class CreateGuestRequest
{
    public GuestDTO Data = null!;
}
```

---

### 5.1.5 Definição da Port da Aplicação  
A port define o contrato que a camada de aplicação expõe.

```csharp
using Application.Guest.Requests;
using Application.Guest.Responses;

namespace Application.Ports;

public interface IGuestManager
{
    Task<GuestResponse> CreateGuest(CreateGuestRequest request);
}
```

---

### 5.1.6 Implementação do Caso de Uso **GuestManager**

```csharp
using Application.Guest.DTOs;
using Application.Guest.Requests;
using Application.Guest.Responses;
using Application.Ports;
using Domain.Ports;

namespace Application;

public class GuestManager : IGuestManager
{
    private readonly IGuestRepository _guestRepository;

    public GuestManager(IGuestRepository guestRepository)
    {
        _guestRepository = guestRepository;
    }

    public async Task<GuestResponse> CreateGuest(CreateGuestRequest request)
    {
        try
        {
            var guest = GuestDTO.MapToEntity(request.Data);
            request.Data.Id = await _guestRepository.Create(guest);

            return new GuestResponse
            {
                Data = request.Data,
                Sucess = true
            };
        }
        catch (Exception)
        {
            return new GuestResponse
            {
                Sucess = false,
                ErrorCode = ErrorCodes.COULD_NOT_STORE_DATA,
                Message = "There was an erro when saving to DB"
            };
        }
    }
}
```

---

### 5.1.7 Implementação do Repositório (Adapter)

```csharp
using Domain.Ports;

namespace Data.Guest;

public class GuestRepository : IGuestRepository
{
    private readonly HotelDBContext _hotelDbContext;

    public GuestRepository(HotelDBContext hotelDBContext)
    {
        _hotelDbContext = hotelDBContext;
    }

    public async Task<int> Create(Domain.Entities.Guest guest)
    {
        await _hotelDbContext.Guests.AddAsync(guest);
        await _hotelDbContext.SaveChangesAsync();
        return guest.Id;
    }

    public Task<Domain.Entities.Guest> Get(int id)
    {
        throw new NotImplementedException();
    }
}
```

---

### 5.1.8 Fluxo Completo do Caso de Uso  
1. A API recebe um `GuestDTO`.  
2. A API cria um `CreateGuestRequest`.  
3. O `GuestManager` converte o DTO em entidade.  
4. O repositório persiste o hóspede.  
5. O caso de uso retorna um `GuestResponse` com sucesso ou erro.  

---

### 5.1.9 Boas Práticas Aplicadas  
#### 5.1.9.1 DTOs não pertencem ao domínio  
Eles são exclusivos da camada de aplicação.

#### 5.1.9.2 Value Objects reforçam invariantes  
`PersonId` garante consistência de documentos.

#### 5.1.9.3 Ports garantem isolamento  
A aplicação não conhece detalhes de persistência.

#### 5.1.9.4 Try/Catch encapsula falhas de infraestrutura  
A aplicação retorna erros padronizados.

---

### 5.1.10 Conclusão  
O caso de uso **CreateGuest** demonstra a integração entre DTOs, Value Objects, Ports, Repositórios e Responses, seguindo rigorosamente os princípios de DDD e Hexagonal Architecture. A implementação é modular, testável e preparada para evolução, mantendo o domínio isolado e a aplicação responsável pela orquestração do fluxo.
## 5.2 Adicionando o Mapper

### 5.2.1 Introdução  
A utilização de **AutoMapper** na camada de aplicação reduz código repetitivo, centraliza regras de transformação entre DTOs e entidades e mantém o caso de uso mais limpo. Em arquiteturas Hexagonais e DDD, o mapeamento pertence à **Application Layer**, pois é nela que ocorre a comunicação entre DTOs (interface externa) e entidades (domínio).

Nesta seção, é implementado o **GuestProfile**, responsável por mapear `GuestDTO` ↔ `Guest`, incluindo o Value Object `PersonId`.

---

### 5.2.2 Criando o Profile de Mapeamento  
O profile define como os objetos serão convertidos entre si. O AutoMapper permite mapear propriedades simples e propriedades aninhadas (como Value Objects).

```csharp
using Application.Guest.DTOs;
using AutoMapper;
using Entities = Domain.Entities;

namespace Application.Guest.Mappings;

public class GuestProfile : Profile
{
    public GuestProfile()
    {
        CreateMap<Entities.Guest, GuestDTO>()
            .ForMember(d => d.IdNumber, opt => opt.MapFrom(
                src => src.DocumentId.IdNumber
            ))
            .ForMember(d => d.IdTypeCode, opt => opt.MapFrom(
                src => src.DocumentId.DocumentType
            ))
            .ReverseMap()
            .ForPath(d => d.DocumentId.IdNumber, opt => opt.MapFrom(
                src => src.IdNumber
            ))
            .ForPath(d => d.DocumentId.DocumentType, opt => opt.MapFrom(
                src => src.IdTypeCode
            ));
    }
}
```

### 5.2.3 Análise do Mapeamento  
#### 5.2.3.1 Mapeamento Guest → GuestDTO  
- `DocumentId.IdNumber` → `IdNumber`  
- `DocumentId.DocumentType` → `IdTypeCode`

#### 5.2.3.2 Mapeamento GuestDTO → Guest  
- `IdNumber` → `DocumentId.IdNumber`  
- `IdTypeCode` → `DocumentId.DocumentType`

O uso de **ForPath** é necessário porque `DocumentId` é um Value Object e não uma propriedade simples.

---

### 5.2.4 Atualizando o Caso de Uso para Usar AutoMapper  
O `GuestManager` agora recebe um `IMapper` via injeção de dependência e utiliza o profile configurado.

```csharp
using Application.Guest.Requests;
using Application.Guest.Responses;
using Application.Ports;
using AutoMapper;
using Domain.Ports;
using Entities = Domain.Entities;

namespace Application;

public class GuestManager : IGuestManager
{
    private readonly IGuestRepository _guestRepository;
    private readonly IMapper _mapper;

    public GuestManager(IGuestRepository guestRepository, IMapper mapper)
    {
        _guestRepository = guestRepository;
        _mapper = mapper;
    }

    public async Task<GuestResponse> CreateGuest(CreateGuestRequest request)
    {
        try
        {
            var guest = _mapper.Map<Entities.Guest>(request.Data);
            request.Data.Id = await _guestRepository.Create(guest);

            return new GuestResponse
            {
                Data = request.Data,
                Sucess = true
            };
        }
        catch (Exception)
        {
            return new GuestResponse
            {
                Sucess = false,
                ErrorCode = ErrorCodes.COULD_NOT_STORE_DATA,
                Message = "There was an erro when saving to DB"
            };
        }
    }
}
```

---

### 5.2.5 Registrando o AutoMapper na API  
A API deve registrar o AutoMapper e carregar automaticamente os profiles da camada de aplicação.

```csharp
builder.Services.AddAutoMapper(typeof(GuestProfile).Assembly);
```

Isso garante que todos os profiles do assembly sejam carregados.

---

### 5.2.6 Boas Práticas  
#### 5.2.6.1 Profiles por Contexto  
Cada agregado deve possuir seu próprio profile, mantendo organização e clareza.

#### 5.2.6.2 DTOs Apenas na Camada de Aplicação  
O domínio nunca deve conhecer DTOs ou AutoMapper.

#### 5.2.6.3 Value Objects Devem Ser Mapeados Explicitamente  
AutoMapper não infere automaticamente propriedades internas de Value Objects.

---

### 5.2.7 Conclusão  
A adição do AutoMapper simplifica o caso de uso **CreateGuest**, reduz duplicação de código e centraliza regras de transformação entre DTOs e entidades. O mapeamento de Value Objects é tratado de forma explícita, mantendo a integridade do domínio e a clareza da camada de aplicação.
## 5.3 Configurando Injeção de Dependência e Finalizando o Use Case

### 5.3.1 Introdução  
Com o caso de uso **CreateGuest** implementado, o próximo passo é disponibilizá‑lo para a API por meio de **injeção de dependência (IoC)**. Nesta etapa, a API passa a consumir o `GuestManager`, que por sua vez utiliza o `GuestRepository` para persistir dados. Também é configurado o AutoMapper e o pipeline HTTP.

Essa configuração completa o fluxo: **Controller → Use Case → Repository → Database**.

---

### 5.3.2 Implementando o Controller  
O controller recebe o DTO, cria a request e delega o processamento ao caso de uso.

```csharp
using Application;
using Application.Guest.DTOs;
using Application.Guest.Requests;
using Application.Ports;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class GuestController : ControllerBase
{
    private readonly ILogger<GuestController> _logger;
    private readonly IGuestManager _guestManager;

    public GuestController(
        ILogger<GuestController> logger,
        IGuestManager guestManager
    )
    {
        _logger = logger;
        _guestManager = guestManager;
    }

    [HttpPost]
    public async Task<ActionResult<GuestDTO>> Post(GuestDTO guest)
    {
        var request = new CreateGuestRequest { Data = guest };

        var res = await _guestManager.CreateGuest(request);

        if (res.Sucess)
            return Created("", res.Data);

        if (res.ErrorCode == ErrorCodes.NOT_FOUND)
            return BadRequest(res);

        _logger.LogError("Response with unknown ErrorCode Returned {@res}", res);
        return StatusCode(StatusCodes.Status500InternalServerError, res);
    }
}
```

### 5.3.3 Registrando Dependências no Program.cs  
A API precisa registrar:

- AutoMapper  
- Use Case (`IGuestManager`)  
- Repository (`IGuestRepository`)  
- DbContext (`HotelDBContext`)  

```csharp
using Application;
using Application.Ports;
using Data;
using Data.Guest;
using Domain.Ports;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

#region automapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
#endregion

#region IoC
builder.Services.AddScoped<IGuestManager, GuestManager>();
builder.Services.AddScoped<IGuestRepository, GuestRepository>();
#endregion

#region Database Context
var connectionString = builder.Configuration.GetConnectionString("Main");
builder.Services.AddDbContext<HotelDBContext>(options =>
    options.UseNpgsql(connectionString)
);
#endregion

#region OpenAPI Documentation
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
#endregion

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
```

---

### 5.3.4 Fluxo Completo do Caso de Uso  
O fluxo final fica estruturado da seguinte forma:

1. **GuestController** recebe o DTO.  
2. Cria um `CreateGuestRequest`.  
3. Chama `_guestManager.CreateGuest(request)`.  
4. O `GuestManager` usa o AutoMapper para converter DTO → Entidade.  
5. O `GuestRepository` persiste o hóspede no banco.  
6. O caso de uso retorna um `GuestResponse`.  
7. O controller retorna o status HTTP apropriado.

---

### 5.3.5 Boas Práticas Aplicadas  
#### 5.3.5.1 Controllers finos  
Toda lógica de negócio permanece no caso de uso.

#### 5.3.5.2 Injeção de dependência explícita  
Cada componente recebe apenas o que precisa.

#### 5.3.5.3 AutoMapper centralizado  
O mapeamento não polui o caso de uso.

#### 5.3.5.4 Respostas padronizadas  
A API retorna mensagens consistentes e previsíveis.

---

### 5.3.6 Conclusão  
Com a configuração de injeção de dependência, o caso de uso **CreateGuest** está totalmente integrado à API. O fluxo segue rigorosamente os princípios de DDD e Hexagonal Architecture, garantindo separação de responsabilidades, testabilidade e clareza arquitetural. O microserviço agora está apto a receber requisições HTTP e persistir hóspedes no banco de dados de forma consistente.

## 5.4 Adicionando Lógica de Negócio ao Salvar

### 5.4.1 Introdução  
A camada de domínio é responsável por garantir a integridade das regras de negócio. Isso significa que **nenhuma entidade deve ser salva** sem antes validar seus próprios invariantes. Nesta etapa, a entidade `Guest` passa a controlar sua lógica de validação e persistência, reforçando os princípios de DDD:

- O domínio protege suas invariantes.  
- O domínio não confia em dados externos.  
- O domínio decide quando e como pode ser persistido.  

Essa abordagem mantém a aplicação mais limpa e garante que erros de negócio sejam detectados antes de atingir a infraestrutura.

---

### 5.4.2 Adicionando Validações na Entidade Guest  
A entidade agora valida:

- Documento válido  
- Campos obrigatórios  
- Email válido  

```csharp
using Domain.Enums;
using Domain.Exceptions;
using Domain.Ports;
using Domain.ValueObjects;

namespace Domain.Entities
{
    public class Guest
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Surname { get; set; }
        public required string Email { get; set; }
        public PersonId DocumentId { get; set; } = null!;

        private void ValidateStatus()
        {
            if (
                DocumentId == null ||
                DocumentId.IdNumber.Length <= 3 ||
                Enum.IsDefined(typeof(DocumentTypes), DocumentId.DocumentType) == false
            )
            {
                throw new InvalidPersonDucumentIdException();
            }

            if (Name == null || Surname == null || Email == null)
            {
                throw new MissingRequiredInformation();
            }

            if (!Utils.ValidatEmail(Email))
            {
                throw new InvalidEmailException();
            }
        }

        public async Task Save(IGuestRepository guestRepository)
        {
            ValidateStatus();

            if (Id == 0)
            {
                Id = await guestRepository.Create(this);
            }
            else
            {
                // await guestRepository.Update(this);
            }
        }
    }
}
```

---

### 5.4.3 Utilitário de Validação  
```csharp
namespace Domain;

public class Utils
{
    public static bool ValidatEmail(string Email)
    {
        if (Email == "b@b.com") return false;

        return true;
    }
}
```

---

### 5.4.4 Exceções de Domínio  
Cada erro de validação lança uma exceção específica.

```csharp
namespace Domain.Exceptions;

public class InvalidEmailException : Exception { }

public class InvalidPersonDucumentIdException : Exception { }

public class MissingRequiredInformation : Exception { }
```

Essas exceções permitem que a camada de aplicação identifique o tipo de erro e retorne o código apropriado.

---

### 5.4.5 Ajustando o Controller para Tratar Erros de Domínio  
O controller agora reconhece novos códigos de erro:

```csharp
using Application;
using Application.Guest.DTOs;
using Application.Guest.Requests;
using Application.Ports;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class GuestController : ControllerBase
{
    private readonly ILogger<GuestController> _logger;
    private readonly IGuestManager _guestManager;

    public GuestController(
        ILogger<GuestController> logger,
        IGuestManager guestManager
    )
    {
        _logger = logger;
        _guestManager = guestManager;
    }

    [HttpPost]
    public async Task<ActionResult<GuestDTO>> Post(GuestDTO guest)
    {
        var request = new CreateGuestRequest { Data = guest };

        var res = await _guestManager.CreateGuest(request);

        if (res.Sucess) 
            return Created("", res.Data);

        if (
            res.ErrorCode == ErrorCodes.INVALID_PERSON_DOCUMENT ||
            res.ErrorCode == ErrorCodes.MISSING_REQUIRED_INFORMATION ||
            res.ErrorCode == ErrorCodes.INVALID_EMAIL ||
            res.ErrorCode == ErrorCodes.COULD_NOT_STORE_DATA ||
            res.ErrorCode == ErrorCodes.NOT_FOUND
        )
        {
            return BadRequest(res);
        }

        _logger.LogError("Response with unknown ErrorCode Returned {@res}", res);
        return StatusCode(StatusCodes.Status500InternalServerError, res);
    }
}
```

---

### 5.4.6 Boas Práticas Aplicadas  
#### 5.4.6.1 Domínio como Guardião das Regras  
A entidade valida seus próprios dados antes de persistir.

#### 5.4.6.2 Exceções Específicas  
Cada erro de negócio possui sua própria exceção, facilitando o tratamento.

#### 5.4.6.3 Aplicação como Orquestradora  
A camada de aplicação apenas captura erros e retorna respostas adequadas.

#### 5.4.6.4 Controller Fino  
O controller apenas traduz respostas em códigos HTTP.

---

### 5.4.7 Conclusão  
A adição de lógica de negócio ao método `Save` fortalece o domínio e garante que apenas dados válidos sejam persistidos. Essa abordagem segue fielmente os princípios de DDD, mantendo o domínio responsável por suas invariantes e deixando a aplicação responsável pela orquestração do fluxo. O resultado é um microserviço mais robusto, seguro e alinhado à arquitetura Hexagonal.

## 5.5 Escrevendo Tests para o **GuestManager**

### 5.5.1 Introdução  
O caso de uso **CreateGuest** contém lógica de negócio, mapeamento, validações e interação com o repositório. Para garantir que o comportamento esteja correto, é necessário escrever testes unitários que validem:

- Cenários positivos (criação bem‑sucedida).  
- Cenários negativos (dados inválidos, exceções de domínio).  
- Interações com o repositório.  
- Funcionamento do AutoMapper (mockado).  

A abordagem utiliza **Moq** para simular o repositório e o AutoMapper, garantindo que o teste seja isolado da infraestrutura.

---

### 5.5.2 Preparação do Ambiente de Testes  
O pacote Moq é adicionado ao projeto:

```
dotnet add package Moq
```

---

### 5.5.3 Estrutura Completa dos Testes  

```csharp
using Application;
using Application.Guest.DTOs;
using Application.Guest.Requests;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;
using Domain.Ports;
using Domain.ValueObjects;
using Moq;

namespace ApplicationTests
{
    public class Tests
    {
        GuestManager _guestManager;
        int _createdGuestId = 111;

        [SetUp]
        public void Setup()
        {
            var fakeRepository = new Mock<IGuestRepository>();

            fakeRepository.Setup(
                x => x.Create(
                    It.IsAny<Guest>())
            ).Returns(
                Task.FromResult(_createdGuestId)
            );

            var mapperMock = new Mock<IMapper>();
            mapperMock.Setup(
                x => x.Map<Guest>(It.IsAny<GuestDTO>())
            ).Returns((GuestDTO dto) =>
                new Guest
                {
                    Name = dto.Name,
                    Surname = dto.Surname,
                    Email = dto.Email,
                    DocumentId = new PersonId(
                        dto.IdNumber,
                        (DocumentTypes)dto.IdTypeCode
                    )
                }
            );

            _guestManager = new GuestManager(fakeRepository.Object, mapperMock.Object);
        }

        #region TESTES POSITIVOS

        [Test]
        public async Task ShouldCreateGuest()
        {
            var guestDTo = new GuestDTO
            {
                Name = "John",
                Surname = "Doe",
                Email = "john.doe@example.com",
                IdNumber = "123456789",
                IdTypeCode = 1
            };

            var createGuestRequest = new CreateGuestRequest
            {
                Data = guestDTo
            };

            var result = await _guestManager.CreateGuest(createGuestRequest);

            Assert.That(result, Is.Not.Null);
            Assert.That(result.Sucess, Is.True);
            Assert.That(result.Data.Id, Is.EqualTo(_createdGuestId));
        }

        #endregion

        #region TESTES NEGATIVOS

        [TestCase("Jhon", "Doe", "123", 1, ErrorCodes.INVALID_PERSON_DOCUMENT, "john.doe@example.com")]
        [TestCase("Jhon", "Doe", "", 1, ErrorCodes.INVALID_PERSON_DOCUMENT, "john.doe@example.com")]
        [TestCase("Jhon", "Doe", "45656565", 11, ErrorCodes.INVALID_PERSON_DOCUMENT, "john.doe@example.com")]
        [TestCase("Jhon", "Doe", "45656565", 1, ErrorCodes.INVALID_EMAIL, "b@b.com")]
        [TestCase("Jhon", "Doe", "45656565", 1, ErrorCodes.MISSING_REQUIRED_INFORMATION, "")]
        [TestCase("Jhon", "Doe", "45656565", 1, ErrorCodes.MISSING_REQUIRED_INFORMATION, null)]
        [TestCase("", "Doe", "45656565", 1, ErrorCodes.MISSING_REQUIRED_INFORMATION, null)]
        [TestCase("Jhon", " ", "45656565", 1, ErrorCodes.MISSING_REQUIRED_INFORMATION, null)]
        [TestCase("Jhon", "Doe", "", 1, ErrorCodes.INVALID_PERSON_DOCUMENT, null)]
        [TestCase("Jhon", "Doe", null, 1, ErrorCodes.INVALID_PERSON_DOCUMENT, "jhon.doe@example.com")]
        public async Task ShouldNotCreateGuestWithInvalidData(
            string name,
            string surname,
            string? idNumber,
            int idTypeCode,
            ErrorCodes expectedErrorCode,
            string? email
        )
        {
            var guestDTo = new GuestDTO
            {
                Name = name,
                Surname = surname,
                Email = email!,
                IdNumber = idNumber!,
                IdTypeCode = idTypeCode
            };

            var createGuestRequest = new CreateGuestRequest
            {
                Data = guestDTo
            };

            var result = await _guestManager.CreateGuest(createGuestRequest);

            Assert.That(result.ErrorCode, Is.EqualTo(expectedErrorCode));
            Assert.That(result.Sucess, Is.False);
        }

        #endregion
    }
}
```

---

### 5.5.4 O que os testes garantem  
#### 5.5.4.1 Cenários positivos  
- O Guest é criado corretamente.  
- O ID retornado pelo repositório é atribuído ao DTO.  
- O caso de uso retorna sucesso.  

#### 5.5.4.2 Cenários negativos  
- Validações de domínio são acionadas corretamente.  
- Cada exceção gera o `ErrorCode` correspondente.  
- O caso de uso retorna falha sem chamar o repositório.  

#### 5.5.4.3 Mock do AutoMapper  
- O mapeamento é isolado e previsível.  

#### 5.5.4.4 Mock do Repositório  
- O teste não depende de banco de dados.  

---

### 5.5.5 Conclusão  
Os testes do **GuestManager** garantem que o caso de uso funcione corretamente em todos os cenários relevantes. A combinação de Moq, NUnit e DDD permite validar regras de negócio, interações com o repositório e comportamento da aplicação de forma isolada, rápida e confiável.

## 5.6 Implementando o **GetGuest**

### 5.6.1 Introdução  
O caso de uso **GetGuest** complementa o fluxo iniciado com **CreateGuest**, permitindo consultar hóspedes já cadastrados. Essa funcionalidade segue os mesmos princípios arquiteturais aplicados anteriormente:

- A **API** recebe o `guestId`.  
- A **Application Layer** orquestra o fluxo.  
- O **Repository** acessa o banco.  
- O **AutoMapper** converte a entidade para DTO.  
- A **Response** padroniza o retorno.  

A seguir, toda a implementação é detalhada.

---

## 5.6.2 Implementação do Repositório (Adapter)

O repositório agora implementa o método `Get`, retornando um `Guest?`.

```csharp
using Domain.Ports;

namespace Data.Guest;

public class GuestRepository : IGuestRepository
{
    private HotelDBContext _hotelDbContext;

    public GuestRepository(HotelDBContext hotelDBContext)
    {
        _hotelDbContext = hotelDBContext;
    }

    public async Task<int> Create(Domain.Entities.Guest guest)
    {
        await _hotelDbContext.Guests.AddAsync(guest);
        await _hotelDbContext.SaveChangesAsync();
        return guest.Id;
    }

    public async Task<Domain.Entities.Guest?> Get(int id)
    {
        return await _hotelDbContext.Guests.FindAsync(id);
    }
}
```

---

## 5.6.3 Atualizando a Port do Domínio

```csharp
using Domain.Entities;

namespace Domain.Ports
{
    public interface IGuestRepository
    {
        Task<Guest?> Get(int id);
        Task<int> Create(Guest guest);
    }
}
```

---

## 5.6.4 Atualizando o Controller

O controller agora expõe o endpoint GET:

```csharp
[HttpGet]
public async Task<ActionResult<GuestDTO>> Get(int guestId)
{
    var res = await _guestManager.GetGuest(guestId);

    if(res.Sucess) return Ok(res.Data);

    if(res.ErrorCode == ErrorCodes.NOT_FOUND)
    {
        return NotFound(res);
    }

    _logger.LogError("Response with unknown ErrorCode Returned{@res}", res);
    return StatusCode(StatusCodes.Status500InternalServerError, res);
}
```

---

## 5.6.5 Atualizando a Port da Aplicação

```csharp
using Application.Guest.Requests;
using Application.Guest.Responses;

namespace Application.Ports;

public interface IGuestManager
{
    Task<GuestResponse> CreateGuest(CreateGuestRequest request);
    Task<GuestResponse> GetGuest(int guestId);
}
```

---

## 5.6.6 Implementação do Caso de Uso **GetGuest**

A implementação segue o mesmo padrão de tratamento de erros e uso do AutoMapper.

```csharp
public async Task<GuestResponse> GetGuest(int guestId)
{
    try
    {
        var guest = await _guestRepository.Get(guestId);

        if (guest == null)
        {
            return new GuestResponse
            {
                Sucess = false,
                ErrorCode = ErrorCodes.NOT_FOUND,
                Message = "Guest not found"
            };
        }

        var dto = _mapper.Map<GuestDTO>(guest);

        return new GuestResponse
        {
            Sucess = true,
            Data = dto
        };
    }
    catch (Exception)
    {
        return new GuestResponse
        {
            Sucess = false,
            ErrorCode = ErrorCodes.COULD_NOT_STORE_DATA,
            Message = "Unexpected error while retrieving guest"
        };
    }
}
```

---

## 5.6.7 Testes Unitários

### 5.6.7.1 Teste Positivo

```csharp
[Test]
public async Task ShouldGetGuest()
{
    var result = await _guestManager.GetGuest(_createdGuestId);

    Assert.That(result, Is.Not.Null);
    Assert.That(result.Sucess, Is.True);
    Assert.That(result.Data.Id, Is.EqualTo(_createdGuestId));
}
```

### 5.6.7.2 Teste Negativo – Guest não encontrado

```csharp
[TestCase]
public async Task ShoulReturnNotFoundWhenTryingToGetNonExistingGuest()
{
    var result = await _guestManager.GetGuest(-1);

    Assert.That(result.Sucess, Is.False);
    Assert.That(result.ErrorCode, Is.EqualTo(ErrorCodes.NOT_FOUND));
}
```

---

## 5.6.8 Boas Práticas Aplicadas

### 5.6.8.1 Repositório retorna `null` quando não encontra  
Evita exceções desnecessárias.

### 5.6.8.2 Caso de uso traduz `null` → `NOT_FOUND`  
Mantém consistência nas respostas.

### 5.6.8.3 AutoMapper converte entidade → DTO  
Evita duplicação de código.

### 5.6.8.4 Controller retorna códigos HTTP adequados  
- `200 OK`  
- `404 Not Found`  
- `500 Internal Server Error`  

---

## 5.6.9 Conclusão  
A implementação do **GetGuest** completa o ciclo CRUD inicial do agregado Guest. O fluxo segue rigorosamente os princípios de DDD e Hexagonal Architecture, mantendo:

- Domínio isolado  
- Aplicação como orquestradora  
- Infraestrutura plugável  
- Testes unitários garantindo comportamento  

O microserviço agora possui um caso de uso robusto para consulta de hóspedes.
