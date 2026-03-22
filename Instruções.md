
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

# 6 Feature Room

## **6.1 Implementando a Feature Room**

### **6.1.1 Introdução**  
A Feature **Room** adiciona ao sistema a capacidade de cadastrar e consultar quartos disponíveis no hotel. Assim como na Feature Guest, seguimos rigorosamente os princípios de **DDD**, **Arquitetura Hexagonal**, **SOLID** e **TDD**, garantindo:

- Domínio rico com validações internas  
- Value Objects (Price)  
- Exceções específicas  
- Ports e Adapters isolados  
- Use Cases na camada Application  
- Mapeamento com AutoMapper  
- Controller fino e orientado a HTTP  
- Testes unitários independentes  

Esta seção descreve a implementação completa da Feature Room.

---

### **6.1.2 Estrutura do Domínio**

#### **6.1.2.1 Entidade Room**

```csharp
namespace Domain.Room;

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

    public void ValdateStatus()
    {
        if (string.IsNullOrWhiteSpace(Name))
            throw new InvalidRoomDataException();

        if (Price == null || Price.Value <= 0 ||
            !Enum.IsDefined(typeof(Enums.AcceptedCurrencies), Price.Currency))
            throw new InvalidRoomPriceException();

        if (Level < 0)
            throw new InvalidRoomLevelException();
    }

    public async Task Save(IRoomRepository roomRepository)
    {
        ValdateStatus();

        if (Id == 0)
            Id = await roomRepository.Create(this);
        else
        {
            // await roomRepository.Update(this);
        }
    }
}
```

#### **6.1.2.2 Value Object Price**

```csharp
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

#### **6.1.2.3 Exceções do Domínio**

```csharp
public class InvalidRoomDataException : Exception { }
public class InvalidRoomPriceException : Exception { }
public class InvalidRoomLevelException : Exception { }
```

---

### **6.1.3 Ports (Interfaces do Domínio)**

```csharp
namespace Domain.Room.Ports;

public interface IRoomRepository
{
    Task<int> Create(Room room);
    Task<Room?> Get(int id);
}
```

---

### **6.1.4 Adapter de Persistência (Infra/Data)**

```csharp
public class RoomRepostory : IRoomRepository
{
    private HotelDBContext _hotelDbContext;

    public RoomRepostory(HotelDBContext hotelDBContext)
    {
        _hotelDbContext = hotelDBContext;
    }

    public async Task<int> Create(Room room)
    {
        await _hotelDbContext.Rooms.AddAsync(room);
        await _hotelDbContext.SaveChangesAsync();
        return room.Id;
    }

    public async Task<Room?> Get(int id)
    {
        return await _hotelDbContext.Rooms.FindAsync(id);
    }
}
```

---

### **6.1.5 DTOs e Requests (Application Layer)**

#### **6.1.5.1 RoomDTO**

```csharp
public class RoomDTO
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int Level { get; set; }
    public bool InMaintenance { get; set; }
    public decimal Value { get; set; }
    public int Currency { get; set; }
}
```

#### **6.1.5.2 CreateRoomRequest**

```csharp
public class CreateRoomRequest
{
    public RoomDTO Data { get; set; } = null!;
}
```

---

### **6.1.6 Mapeamento com AutoMapper**

```csharp
public class RoomProfile : Profile
{
    public RoomProfile()
    {
        CreateMap<Domain.Room.Room, RoomDTO>()
            .ForMember(d => d.Currency, opt => opt.MapFrom(src => src.Price.Currency))
            .ForMember(d => d.Value, opt => opt.MapFrom(src => src.Price.Value))
            .ReverseMap()
                .ForPath(d => d.Price.Currency, opt => opt.MapFrom(src => src.Currency))
                .ForPath(d => d.Price.Value, opt => opt.MapFrom(src => src.Value));
    }
}
```

---

### **6.1.7 Caso de Uso RoomManager**

#### **6.1.7.1 CreateRoom**

```csharp
public async Task<RoomResponse> CreateRoom(CreateRoomRequest request)
{
    try
    {
        var room = _mapper.Map<Domain.Room.Room>(request.Data);
        await room.Save(_roomRepository);

        request.Data.Id = room.Id;

        return new RoomResponse
        {
            Data = request.Data,
            Success = true,
        };
    }
    catch (InvalidRoomDataException)
    {
        return new RoomResponse
        {
            Success = false,
            ErrorCode = ErrorCodes.MISSING_REQUIRED_INFORMATION_ROOM,
            Message = "Missing required information to create a room"
        };
    }
    catch (InvalidRoomPriceException)
    {
        return new RoomResponse
        {
            Success = false,
            ErrorCode = ErrorCodes.INVALID_ROOM_PRICE,
            Message = "The price provided for the room is invalid"
        };
    }
    catch (InvalidRoomLevelException)
    {
        return new RoomResponse
        {
            Success = false,
            ErrorCode = ErrorCodes.INVALID_ROOM_LEVEL,
            Message = "The level provided for the room is invalid"
        };
    }
    catch (Exception)
    {
        return new RoomResponse
        {
            Success = false,
            ErrorCode = ErrorCodes.ROOM_COULD_NOT_STORE_DATA,
            Message = "An error ocurred while creating the room"
        };
    }
}
```

#### **6.1.7.2 GetRoom**

```csharp
public async Task<RoomResponse> GetRoom(int roomId)
{
    try
    {
        var room = await _roomRepository.Get(roomId);

        if (room == null)
        {
            return new RoomResponse
            {
                Success = false,
                ErrorCode = ErrorCodes.NOT_FOUND_ROOM,
                Message = "Room not found"
            };
        }

        var dto = _mapper.Map<RoomDTO>(room);

        return new RoomResponse
        {
            Success = true,
            Data = dto
        };
    }
    catch (Exception)
    {
        return new RoomResponse
        {
            Success = false,
            ErrorCode = ErrorCodes.ROOM_COULD_NOT_STORE_DATA,
            Message = "Unexpected error while retrieving room"
        };
    }
}
```

---

### **6.1.8 Controller REST**

```csharp
[ApiController]
[Route("[controller]")]
public class RoomController : ControllerBase
{
    private readonly ILogger<RoomController> _logger;
    private readonly IRoomManager _roomManager;

    public RoomController(ILogger<RoomController> logger, IRoomManager roomManager)
    {
        _logger = logger;
        _roomManager = roomManager;
    }

    [HttpPost]
    public async Task<ActionResult<RoomDTO>> Post(RoomDTO room)
    {
        var request = new CreateRoomRequest { Data = room };
        var res = await _roomManager.CreateRoom(request);

        if (res.Success) return Created("", res.Data);

        if (res.ErrorCode == ErrorCodes.ROOM_COULD_NOT_STORE_DATA)
            return BadRequest(res);

        return StatusCode(500, res);
    }

    [HttpGet]
    public async Task<ActionResult<RoomDTO>> Get(int roomId)
    {
        var res = await _roomManager.GetRoom(roomId);

        if (res.Success) return Ok(res.Data);

        if (res.ErrorCode == ErrorCodes.NOT_FOUND_ROOM)
            return NotFound(res);

        return StatusCode(500, res);
    }
}
```

---

### **6.1.9 Injeção de Dependência**

```csharp
builder.Services.AddScoped<IRoomManager, RoomManager>();
builder.Services.AddScoped<IRoomRepository, RoomRepostory>();
```

---

### **6.1.10 Conclusão**

A Feature Room foi implementada seguindo rigorosamente os princípios de DDD e Arquitetura Hexagonal. O domínio controla suas regras, a aplicação orquestra o fluxo, a infraestrutura persiste os dados e a API expõe endpoints REST limpos e previsíveis.  

A base está sólida para evoluir para:

- Testes unitários (6.2)  
- Atualização de quartos  
- Listagem paginada  
- Integração com Booking  

## **6.2 Testando a Camada Application – Room**

### **6.2.1 Introdução**  
Com a Feature Room implementada, é essencial garantir que o caso de uso **RoomManager** funcione corretamente em todos os cenários.  
Nesta etapa, aplicamos **TDD** e **testes unitários isolados**, utilizando:

- **Moq** para simular o repositório (`IRoomRepository`)
- **Moq** para simular o AutoMapper
- **NUnit** como framework de testes
- Testes positivos e negativos
- Validação de regras de domínio via exceções

O objetivo é garantir que:

- Quartos válidos sejam criados corretamente  
- Erros de domínio sejam capturados e convertidos em `ErrorCodes`  
- O método `GetRoom` retorne corretamente quartos existentes  
- O método `GetRoom` retorne NOT_FOUND quando necessário  

---

### **6.2.2 Preparação do Ambiente de Testes**

Antes de tudo, adicionamos o pacote Moq:

```
dotnet add package Moq
```

Em seguida, configuramos o ambiente de testes criando mocks para:

- `IRoomRepository`
- `IMapper`

---

### **6.2.3 Estrutura Completa dos Testes**

```csharp
using Application;
using Application.Room;
using Application.Room.DTOs;
using Application.Room.Requests;
using AutoMapper;
using Domain.Room;
using Domain.Room.Ports;
using Moq;

namespace ApplicationTests;

public class RoomManagerTests
{
    RoomManager _roomManager;
    int _createdRoomId = 111;

    [SetUp]
    public void Setup()
    {
        var fakeRepository = new Mock<IRoomRepository>();

        fakeRepository.Setup(
            x => x.Create(
                It.IsAny<Room>())
        ).Returns(
            Task.FromResult(_createdRoomId)
        );

        fakeRepository.Setup(
            x => x.Get(_createdRoomId)
        ).ReturnsAsync(
            new Room
            {
                Id = _createdRoomId,
                Name = "Room 101",
                Level = 15,
                InMaintenance = false,
                Price = new Domain.ValueObjects.Price
                {
                    Value = 100,
                    Currency = Domain.Enums.AcceptedCurrencies.Dollar
                }
            }
        );

        var mapperMock = new Mock<IMapper>();

        mapperMock.Setup(
            x => x.Map<Room>(It.IsAny<RoomDTO>())
        ).Returns((RoomDTO dto) =>
            new Room
            {
                Name = dto.Name,
                Level = dto.Level,
                InMaintenance = dto.InMaintenance,
                Price = new Domain.ValueObjects.Price
                {
                    Value = dto.Value,
                    Currency = (Domain.Enums.AcceptedCurrencies)dto.Currency
                }
            }
        );

        mapperMock.Setup(
            x => x.Map<RoomDTO>(It.IsAny<Room>())
        ).Returns((Room room) =>
            new RoomDTO
            {
                Name = room.Name,
                Level = room.Level,
                InMaintenance = room.InMaintenance,
                Value = room.Price.Value,
                Currency = (int)room.Price.Currency
            }
        );

        _roomManager = new RoomManager(fakeRepository.Object, mapperMock.Object);
    }

    #region TESTES POSITIVOS

    [Test]
    public async Task Should_Return_Created_RoomId()
    {
        var roomDto = new RoomDTO
        {
            Name = "Room 101",
            Level = 15,
            InMaintenance = false,
            Value = 100,
            Currency = (int)Domain.Enums.AcceptedCurrencies.Dollar
        };

        var createRoomRequest = new CreateRoomRequest
        {
            Data = roomDto
        };

        var result = await _roomManager.CreateRoom(createRoomRequest);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Data.Id, Is.EqualTo(_createdRoomId));
        Assert.That(result.Success, Is.True);
    }

    #endregion

    #region TESTES NEGATIVOS

    [TestCase(null, 15, false, 100, 1, ErrorCodes.MISSING_REQUIRED_INFORMATION_ROOM)]
    [TestCase("Jhon", 15, false, -100, 1, ErrorCodes.INVALID_ROOM_PRICE)]
    [TestCase("Jhon", 15, false, 100, -15, ErrorCodes.INVALID_ROOM_PRICE)]
    [TestCase("Jhon", -15, false, 100, 1, ErrorCodes.INVALID_ROOM_LEVEL)]
    public async Task Should_Return_Error_When_Creating_Room_With_Invalid_Data(
        string? name,
        int level,
        bool inMaintenance,
        decimal value,
        int currency,
        ErrorCodes expectedErrorCode)
    {
        var roomDto = new RoomDTO
        {
            Name = name!,
            Level = level,
            InMaintenance = inMaintenance,
            Value = value,
            Currency = currency
        };

        var createRoomRequest = new CreateRoomRequest
        {
            Data = roomDto
        };

        var result = await _roomManager.CreateRoom(createRoomRequest);

        Assert.That(result, Is.Not.Null);
        Assert.That(result.Success, Is.False);
        Assert.That(result.ErrorCode, Is.EqualTo(expectedErrorCode));
    }

    #endregion
}
```

---

### **6.2.4 Análise dos Testes**

#### **6.2.4.1 Testes Positivos**
O teste `Should_Return_Created_RoomId` garante que:

- O RoomManager chama o repositório corretamente  
- O ID retornado é atribuído ao DTO  
- O fluxo retorna `Success = true`  

#### **6.2.4.2 Testes Negativos**
Os testes negativos validam:

- Nome inválido → `MISSING_REQUIRED_INFORMATION_ROOM`
- Preço inválido → `INVALID_ROOM_PRICE`
- Moeda inválida → `INVALID_ROOM_PRICE`
- Level inválido → `INVALID_ROOM_LEVEL`

Esses testes garantem que as exceções de domínio são capturadas e traduzidas corretamente para `ErrorCodes`.

---

### **6.2.5 Conclusão**

A suíte de testes da Feature Room valida completamente o comportamento do caso de uso **RoomManager**, garantindo:

- Conformidade com regras de negócio  
- Tratamento adequado de exceções  
- Mapeamento correto entre DTO ↔ Entidade  
- Interação correta com o repositório  
- Robustez e previsibilidade do fluxo  

Com isso, a Feature Room está totalmente validada e pronta para integração com a Feature Booking.

---

# 7 Feature Booking
---

## **7.1 Feature Booking (Domain e Application)**

### **7.1.1 Introdução**  
A Feature **Booking** é a mais complexa do sistema até o momento, pois integra três agregados distintos:

- **Guest**
- **Room**
- **Booking**

Ela exige validações cruzadas, regras de negócio específicas e verificação de conflitos de reserva.  
Nesta etapa, seguimos rigorosamente os princípios de:

- **DDD (Domain‑Driven Design)**
- **Arquitetura Hexagonal**
- **SOLID**
- **Clean Architecture**

Garantindo:

- Domínio rico e isolado  
- Regras de negócio encapsuladas  
- Validações obrigatórias  
- Ports e Adapters bem definidos  
- Use Cases claros  
- Mapeamento com AutoMapper  
- Tratamento de exceções específico  
- Respostas padronizadas  

---

### **7.1.2 Estrutura do Domínio Booking**

#### **7.1.2.1 Entidade Booking**

```csharp
namespace Domain.Booking.Entities;

public class Booking
{
    public int Id { get; set; }
    public DateTime PlacedAt { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    private Status Status { get; set; }

    public R.Room Room { get; set; } = null!;
    public int RoomId { get; set; }

    public G.Guest Guest { get; set; } = null!;
    public int GuestId { get; set; }

    public Status CurrentStatus => Status;

    public Booking()
    {
        Status = Status.Created;
        PlacedAt = DateTime.UtcNow;
    }

    public void ChangeState(Action action)
    {
        Status = (Status, action) switch
        {
            (Status.Created, Action.Pay) => Status.Paid,
            (Status.Created, Action.Cancel) => Status.Canceled,
            (Status.Paid, Action.Finish) => Status.Finished,
            (Status.Paid, Action.Refound) => Status.Refounded,
            (Status.Canceled, Action.Reopen) => Status.Created,
            _ => Status
        };
    }

    private async Task Validate(
        IGuestRepository guestRepository,
        IRoomRepository roomRepository,
        IBookingRepository bookingRepository)
    {
        if (PlacedAt == default ||
            Start == default ||
            End == default ||
            GuestId == default ||
            RoomId == default)
        {
            throw new MissingRequiredInformation();
        }

        if (Start >= End)
        {
            throw new InvalidBookingDatesException();
        }

        var guest = await guestRepository.Get(GuestId);
        if (guest == null)
        {
            throw new InvalidGuestIDException();
        }

        var room = await roomRepository.Get(RoomId);
        if (room == null)
        {
            throw new InvalidRoomIDException();
        }

        var hasConflictingBooking =
            await bookingRepository.ExistsActiveBookingForRoom(RoomId, Start, End);

        if (hasConflictingBooking)
        {
            throw new ConflictingBookingException();
        }

        guest.Isvalid();
        room.CanBeBooked();
    }

    public async Task Save(
        IBookingRepository bookingRepository,
        IGuestRepository guestRepository,
        IRoomRepository roomRepository)
    {
        await Validate(guestRepository, roomRepository, bookingRepository);

        if (Id == 0)
        {
            Id = await bookingRepository.Create(this);
        }
        else
        {
            // await bookingRepository.Update(this);
        }
    }
}
```

---

#### **7.1.2.2 Exceções do Domínio Booking**

```csharp
public class MissingRequiredInformation : Exception { }
public class InvalidBookingDatesException : Exception { }
public class ConflictingBookingException : Exception { }
public class InvalidGuestIDException : Exception { }
public class InvalidRoomIDException : Exception { }
```

---

#### **7.1.2.3 Ports do Domínio**

```csharp
public interface IBookingRepository
{
    Task<Booking?> Get(int id);
    Task<int> Create(Booking booking);
    Task<bool> ExistsActiveBookingForRoom(int roomId, DateTime start, DateTime end);
}
```

---

### **7.1.3 DTOs da Camada Application**

#### **7.1.3.1 CreateBookingDTO**

```csharp
public class CreateBookingDTO
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public int RoomId { get; set; }
    public int GuestId { get; set; }
}
```

---

#### **7.1.3.2 ReturnBookingDTO**

```csharp
public class ReturnBookingDTO
{
    public int Id { get; set; }
    public DateTime PlacedAt { get; set; }
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public ReturnRoomDTO Room { get; set; } = null!;
    public ReturnGuestDTO Guest { get; set; } = null!;
}
```

---

### **7.1.4 Requests e Responses**

#### **7.1.4.1 CreateBookingRequest**

```csharp
public class CreateBookingRequest
{
    public CreateBookingDTO Data { get; set; } = null!;
}
```

---

#### **7.1.4.2 BookingResponse**

```csharp
public class BookingResponse : Response
{
    public ReturnBookingDTO Data { get; set; } = null!;
}
```

---

### **7.1.5 Mapeamento com AutoMapper**

#### **7.1.5.1 CreateBookingProfile**

```csharp
public class CreateBookingProfile : Profile
{
    public CreateBookingProfile()
    {
        CreateMap<CreateBookingDTO, Domain.Booking.Entities.Booking>();
    }
}
```

---

#### **7.1.5.2 ReturnBookingProfile**

```csharp
public class ReturnBookingProfile : Profile
{
    public ReturnBookingProfile()
    {
        CreateMap<Domain.Booking.Entities.Booking, ReturnBookingDTO>();
    }
}
```

---

### **7.1.6 Caso de Uso BookingManager**

#### **7.1.6.1 CreateBooking**

```csharp
public async Task<BookingResponse> CreateBooking(CreateBookingRequest request)
{
    try
    {
        var booking = _mapper.Map<Domain.Booking.Entities.Booking>(request.Data);

        await booking.Save(_bookingRepository, _guestRepository, _roomRepository);

        return new BookingResponse
        {
            Data = _mapper.Map<ReturnBookingDTO>(booking),
            Success = true,
        };
    }
    catch (MissingRequiredInformation)
    {
        return new BookingResponse
        {
            Success = false,
            ErrorCode = ErrorCodes.MISSING_REQUIRED_INFORMATION_BOOKING,
            Message = "Some required information for creating the booking was not provided"
        };
    }
    catch (InvalidBookingDatesException)
    {
        return new BookingResponse
        {
            Success = false,
            ErrorCode = ErrorCodes.INVALID_DATES,
            Message = "The provided booking dates are invalid"
        };
    }
    catch (ConflictingBookingException)
    {
        return new BookingResponse
        {
            Success = false,
            ErrorCode = ErrorCodes.CONFLICTING_BOOKING,
            Message = "The provided room is not available for the selected dates"
        };
    }
    catch (Domain.Guest.Exceptions.GuestExceptons)
    {
        return new BookingResponse
        {
            Success = false,
            ErrorCode = ErrorCodes.INVALID_DATA_GUEST,
            Message = "The provided guest has invalid data"
        };
    }
    catch (Domain.Room.Exceptions.RoomExceptions)
    {
        return new BookingResponse
        {
            Success = false,
            ErrorCode = ErrorCodes.INVALID_DATA_ROOM,
            Message = "The provided room has invalid data"
        };
    }
    catch (InvalidGuestIDException)
    {
        return new BookingResponse
        {
            Success = false,
            ErrorCode = ErrorCodes.INVALID_GUEST_ID,
            Message = "The provided guest has invalid data"
        };
    }
    catch (InvalidRoomIDException)
    {
        return new BookingResponse
        {
            Success = false,
            ErrorCode = ErrorCodes.INVALID_ROOM_ID,
            Message = "The provided room does not exist"
        };
    }
    catch (Exception)
    {
        return new BookingResponse
        {
            Success = false,
            ErrorCode = ErrorCodes.BOOKING_COULD_NOT_BE_CREATED,
            Message = "An error occurred while creating the booking"
        };
    }
}
```

---

#### **7.1.6.2 GetBooking**

```csharp
public async Task<BookingResponse> GetBooking(int bookingId)
{
    var booking = await _bookingRepository.Get(bookingId);

    if (booking == null)
    {
        return new BookingResponse
        {
            Success = false,
            ErrorCode = ErrorCodes.NOT_FOUND,
            Message = "No booking found with the provided id"
        };
    }

    return new BookingResponse
    {
        Data = _mapper.Map<ReturnBookingDTO>(booking),
        Success = true,
    };
}
```

---

### **7.1.7 Conclusão**

A Feature Booking integra múltiplos agregados e representa o fluxo mais complexo do sistema até agora.  
A implementação segue rigorosamente os princípios de DDD e Arquitetura Hexagonal:

- O domínio valida todas as regras  
- A aplicação orquestra o fluxo  
- A infraestrutura persiste os dados  
- O AutoMapper converte DTOs e entidades  
- Exceções são traduzidas para códigos de erro claros  

Com isso, a base está pronta para:

- **7.2 Testando Application Booking**  
- **7.3 Implementando BookingRepository**  
- **7.4 Criando BookingController**
---

## **7.2 Feature Booking (Controller e Repository)**

### **7.2.1 Introdução**  
Nesta etapa, concluímos a Feature Booking implementando:

- O **BookingController**, responsável por expor os endpoints REST para criação e consulta de reservas.
- O **BookingRepository**, responsável por persistir reservas e verificar conflitos de datas.
- A configuração do **HotelDBContext** para incluir o mapeamento de Booking.
- A configuração de **EF Core** para armazenar o estado da reserva como string.

Essa camada completa o fluxo da Feature Booking, conectando Application → Domain → Data → API.

---

### **7.2.2 BookingController**

O controller segue o padrão já estabelecido no projeto:

- Fino, sem lógica de negócio
- Converte DTO → Request
- Retorna códigos HTTP adequados
- Registra logs para erros inesperados

```csharp
using Application;
using Application.Booking.DTOs;
using Application.Booking.Ports;
using Application.Booking.Requests;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class BookingController: ControllerBase
{
    private readonly ILogger<BookingController> _logger;
    private readonly IBookingManager _bookingManager;

    public BookingController(
        ILogger<BookingController> logger,
        IBookingManager bookingManager
    )
    {
        _logger = logger;
        _bookingManager = bookingManager;
    }

    [HttpPost]
    public async Task<ActionResult<ReturnBookingDTO>> Post(CreateBookingDTO booking)
    {
        var request = new CreateBookingRequest { Data = booking };
        
        var res = await _bookingManager.CreateBooking(request);

        if(res.Success) return Created("", res.Data);

        if(
            res.ErrorCode == ErrorCodes.INVALID_DATES || 
            res.ErrorCode == ErrorCodes.MISSING_REQUIRED_INFORMATION_BOOKING || 
            res.ErrorCode == ErrorCodes.INVALID_GUEST_ID ||
            res.ErrorCode == ErrorCodes.INVALID_ROOM_ID ||
            res.ErrorCode == ErrorCodes.BOOKING_COULD_NOT_BE_CREATED ||
            res.ErrorCode == ErrorCodes.NOT_FOUND
        )
        {
            return BadRequest(res);
        }

        _logger.LogError("Response with unknown ErrorCode Returned{@res}", res);
        return StatusCode(StatusCodes.Status500InternalServerError, res);
    }

    [HttpGet]
    public async Task<ActionResult<ReturnBookingDTO>> Get(int bookingId)
    {
        var res = await _bookingManager.GetBooking(bookingId);

        if(res.Success) return Ok(res.Data);

        if(res.ErrorCode == ErrorCodes.NOT_FOUND)
        {
            return NotFound(res);
        }

        _logger.LogError("Response with unknown ErrorCode Returned{@res}", res);
        return StatusCode(StatusCodes.Status500InternalServerError, res);
    }
}
```

---

### **7.2.3 Configuração EF Core – BookingConfiguration**

O estado da reserva (`Status`) é armazenado como string no banco, garantindo legibilidade e evitando problemas de enum versioning.

```csharp
using Entities = Domain.Booking.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Domain.Booking.Enums;

namespace Data.Booking;

public class BookingConfigurantion : IEntityTypeConfiguration<Entities.Booking>
{
    public void Configure(EntityTypeBuilder<Entities.Booking> builder)
    {
        builder.HasKey(x=> x.Id);

        builder
            .Property<Status>("Status")
            .HasConversion<string>()
            .IsRequired();
    }
}
```

---

### **7.2.4 BookingRepository (Adapter de Persistência)**

O repositório implementa:

- **Create** → salva a reserva
- **Get** → retorna reserva com Guest e Room carregados
- **ExistsActiveBookingForRoom** → verifica conflitos de datas

```csharp
using Domain.Booking.Ports;
using Microsoft.EntityFrameworkCore;

namespace Data.Booking;

public class BookingRepository : IBookingRepository
{
    private HotelDBContext _hotelDbContext;

    public BookingRepository(HotelDBContext hotelDBContext)
    {
        _hotelDbContext = hotelDBContext;
    }

    public async Task<int> Create(Domain.Booking.Entities.Booking booking)
    {
        await _hotelDbContext.Bookings.AddAsync(booking);
        await _hotelDbContext.SaveChangesAsync();
        return booking.Id;
    }

    public async Task<bool> ExistsActiveBookingForRoom(int roomId, DateTime start, DateTime end)
    {
        return await _hotelDbContext.Bookings.AnyAsync(
            b => b.RoomId == roomId &&
            (
                b.End > start &&
                b.Start < end
            )
        );
    }

    public async Task<Domain.Booking.Entities.Booking?> Get(int id)
    {
        return await _hotelDbContext.Bookings
            .Include(b => b.Guest)
            .Include(b => b.Room)
            .FirstOrDefaultAsync(b => b.Id == id);
    }
}
```

---

### **7.2.5 Atualização do HotelDBContext**

O contexto agora inclui:

- DbSet de Booking
- Aplicação da configuração BookingConfiguration

```csharp
using Microsoft.EntityFrameworkCore;
using Entities_Guest = Domain.Guest.Entities;
using Entities_Room = Domain.Room.Entities;
using Entities_Booking = Domain.Booking.Entities;
using Data.Booking;

namespace Data
{
    public class HotelDBContext(DbContextOptions<HotelDBContext> options) : DbContext(options)
    {
        public virtual DbSet<Entities_Guest.Guest> Guests { get; set; }
        public virtual DbSet<Entities_Room.Room> Rooms { get; set; }
        public virtual DbSet<Entities_Booking.Booking> Bookings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new GuestConfiguration());
            modelBuilder.ApplyConfiguration(new RoomConfiguration());
            modelBuilder.ApplyConfiguration(new BookingConfigurantion());
        }
    }
}
```

---

### **7.2.6 Conclusão**

Com o BookingController e o BookingRepository implementados, a Feature Booking está totalmente integrada ao sistema:

- A API expõe endpoints REST completos
- A camada Application orquestra o fluxo
- O domínio valida regras complexas
- A infraestrutura persiste reservas e verifica conflitos
- O EF Core está configurado para mapear corretamente o estado da reserva

A Feature Booking agora está pronta para:

- **7.3 Testando Application Booking**
- **7.4 Implementando BookingController Tests**
- **7.5 Implementando BookingRepository Tests**
- **7.6 Implementando UpdateBooking / CancelBooking / FinishBooking**


---

## **7.3 Testando Application – BookingManager**

### **7.3.1 Introdução**  
A Feature Booking possui a lógica de negócio mais complexa do sistema, envolvendo:

- Validação de datas  
- Verificação de existência de Guest e Room  
- Regras de disponibilidade de Room  
- Regras de validade de Guest  
- Detecção de conflitos de reservas  
- Mapeamento entre DTOs e entidades  
- Persistência via BookingRepository  

Por isso, os testes unitários do **BookingManager** precisam ser abrangentes, cobrindo:

- Cenários positivos  
- Cenários negativos  
- Conflitos de reserva  
- Dados inválidos  
- IDs inexistentes  
- Interação com múltiplos repositórios  
- AutoMapper real  

Nesta seção, implementamos uma suíte completa de testes utilizando:

- **NUnit**
- **Moq**
- **AutoMapper real com profiles**
- **Store in-memory** para simular conflitos de reserva

---

### **7.3.2 Setup dos Testes**

O setup cria:

- Mocks de `IBookingRepository`, `IRoomRepository`, `IGuestRepository`
- AutoMapper real com todos os profiles necessários
- Um store in-memory para simular reservas persistidas
- Configurações de retorno para cada repositório

```csharp
public class BookingManagerTests
{
    private BookingManager _bookingManager = null!;
    private Mock<IBookingRepository> _bookingRepo = null!;
    private Mock<IRoomRepository> _roomRepo = null!;
    private Mock<IGuestRepository> _guestRepo = null!;
    private IMapper _mapper = null!;
    private readonly int _createdBookingId = 111;
    private readonly int _inAvailableRoomId = 121;
    private readonly int _invalidGuestId = 999;
    private readonly int _invalidRoomId = 999;

    private List<Booking> _store = null!;

    [SetUp]
    public void Setup()
    {
        _bookingRepo = new Mock<IBookingRepository>();
        _roomRepo = new Mock<IRoomRepository>();
        _guestRepo = new Mock<IGuestRepository>();

        _store = new List<Booking>();  // simula persistência

        _bookingRepo
            .Setup(x => x.Create(It.IsAny<Booking>()))
            .ReturnsAsync(_createdBookingId)
            .Callback<Booking>(booking =>
            {
                _store.Add(new Booking
                {
                    RoomId = booking.RoomId,
                    GuestId = booking.GuestId,
                    Start = booking.Start,
                    End = booking.End,
                    PlacedAt = booking.PlacedAt
                });
            });

        _bookingRepo
            .Setup(x => x.Get(_createdBookingId))
            .ReturnsAsync(new Booking
            {
                Id = _createdBookingId,
                PlacedAt = DateTime.UtcNow,
                Start = DateTime.UtcNow.AddDays(1),
                End = DateTime.UtcNow.AddDays(2),
                RoomId = 1,
                GuestId = 1,
                Room = new Room
                {
                    Id = 1,
                    Name = "Room 101",
                    Level = 15,
                    InMaintenance = false,
                    Price = new Price { Value = 100, Currency = AcceptedCurrencies.Dollar }
                },
                Guest = new Guest
                {
                    Id = 1,
                    Name = "John",
                    Surname = "Doe",
                    Email = "john.doe@example.com",
                    DocumentId = new PersonId { IdNumber = "123456789", DocumentType = DocumentTypes.DriverLicense }
                }
            });

        _roomRepo.Setup(x => x.Get(1)).ReturnsAsync(
            new Room
            {
                Id = 1,
                Name = "Room 101",
                Level = 15,
                InMaintenance = false,
                Price = new Price { Value = 100, Currency = AcceptedCurrencies.Dollar }
            });

        _guestRepo.Setup(x => x.Get(1)).ReturnsAsync(
            new Guest
            {
                Id = 1,
                Name = "John",
                Surname = "Doe",
                Email = "john.doe@example.com",
                DocumentId = new PersonId { IdNumber = "123456789", DocumentType = DocumentTypes.DriverLicense }
            });

        _bookingRepo
            .Setup(x => x.ExistsActiveBookingForRoom(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync((int roomId, DateTime start, DateTime end) =>
                _store.Any(b =>
                    b.RoomId == roomId &&
                    b.Start < end &&
                    b.End > start
                )
            );

        _roomRepo.Setup(x => x.Get(_invalidRoomId)).ReturnsAsync(new Room { Id = _invalidRoomId, Name = "Room 101" });
        _guestRepo.Setup(x => x.Get(_invalidGuestId)).ReturnsAsync(new Guest { Id = _invalidGuestId, Name = " ", Surname = "Doe", Email = "john.doe@example.com" });

        _roomRepo.Setup(x => x.Get(_inAvailableRoomId)).ReturnsAsync(new Room
        {
            Id = _inAvailableRoomId,
            Name = "Room 121",
            InMaintenance = true
        });

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<CreateBookingProfile>();
            cfg.AddProfile<ReturnBookingProfile>();
            cfg.AddProfile<CreateRoomProfile>();
            cfg.AddProfile<ReturnRoomProfille>();
            cfg.AddProfile<CreateGuestProfile>();
            cfg.AddProfile<ReturnGuestProfile>();
        });

        _mapper = config.CreateMapper();

        _bookingManager = new BookingManager(
            _bookingRepo.Object,
            _guestRepo.Object,
            _roomRepo.Object,
            _mapper
        );
    }
```

---

### **7.3.3 Testes Positivos**

#### **7.3.3.1 Deve criar uma reserva com sucesso**

```csharp
[Test]
public async Task CreateBooking_Should_Return_Created_Booking()
{
    var createBookingDto = new CreateBookingDTO
    {
        Start = DateTime.UtcNow.AddDays(1),
        End = DateTime.UtcNow.AddDays(3),
        RoomId = 1,
        GuestId = 1
    };

    var request = new CreateBookingRequest { Data = createBookingDto };

    var result = await _bookingManager.CreateBooking(request);

    Assert.That(result, Is.Not.Null);
    Assert.That(result.Success, Is.True);
    Assert.That(_createdBookingId, Is.EqualTo(result.Data.Id));
}
```

---

#### **7.3.3.2 Deve retornar uma reserva existente**

```csharp
[Test]
public async Task ShouldGetBookingById()
{
    var result = await _bookingManager.GetBooking(_createdBookingId);

    Assert.That(result, Is.Not.Null);
    Assert.That(result.Success, Is.True);
    Assert.That(result.Data.Id, Is.EqualTo(_createdBookingId));
    Assert.That(result.Data.Room, Is.Not.Null);
    Assert.That(result.Data.Guest, Is.Not.Null);
}
```

---

### **7.3.4 Testes Negativos**

#### **7.3.4.1 Dados inválidos**

```csharp
[TestCase("2024-01-10", "2024-01-05", 1, 1, ErrorCodes.INVALID_DATES)]
[TestCase("2024-01-10", "2024-01-10", 1, 1, ErrorCodes.INVALID_DATES)]
[TestCase("2024-01-10", "2024-01-15", 995, 1, ErrorCodes.INVALID_ROOM_ID)]
[TestCase("2024-01-10", "2024-01-15", 1, 995, ErrorCodes.INVALID_GUEST_ID)]
[TestCase("2024-01-10", "2024-01-15", 0, 1, ErrorCodes.MISSING_REQUIRED_INFORMATION_BOOKING)]
[TestCase("2024-01-10", "2024-01-15", 1, 0, ErrorCodes.MISSING_REQUIRED_INFORMATION_BOOKING)]
[TestCase("2024-01-10", "2024-01-15", 121, 1, ErrorCodes.INVALID_DATA_ROOM)]
[TestCase("2024-01-10", "2024-01-15", 999, 1, ErrorCodes.INVALID_DATA_ROOM)]
[TestCase("2024-01-10", "2024-01-15", 1, 999, ErrorCodes.INVALID_DATA_GUEST)]
public async Task CreateBooking_Should_Return_Error_For_Invalid_Datas(
    DateTime start,
    DateTime end,
    int roomId,
    int guestId,
    ErrorCodes expectedErrorCode)
{
    var createBookingDto = new CreateBookingDTO
    {
        Start = start,
        End = end,
        RoomId = roomId,
        GuestId = guestId
    };

    var request = new CreateBookingRequest { Data = createBookingDto };

    var result = await _bookingManager.CreateBooking(request);

    Assert.That(result, Is.Not.Null);
    Assert.That(result.Success, Is.False);
    Assert.That(result.ErrorCode, Is.EqualTo(expectedErrorCode));
}
```

---

#### **7.3.4.2 Conflito de reserva**

```csharp
[TestCase("2024-01-10", "2024-01-15")]
[TestCase("2024-01-09", "2024-01-11")]
[TestCase("2024-01-14", "2024-01-16")]
[TestCase("2024-01-09", "2024-01-16")]
[TestCase("2024-01-08", "2024-01-16")]
public async Task CreateBooking_Should_Return_Conflict_When_Second_Booking_Overlaps(DateTime start, DateTime end)
{
    var first = new CreateBookingDTO
    {
        Start = new DateTime(2024, 1, 10),
        End = new DateTime(2024, 1, 15),
        RoomId = 1,
        GuestId = 1
    };

    var firstRes = await _bookingManager.CreateBooking(new CreateBookingRequest { Data = first });
    Assert.That(firstRes.Success, Is.True);

    var second = new CreateBookingDTO
    {
        Start = start,
        End = end,
        RoomId = 1,
        GuestId = 1
    };

    var secondRes = await _bookingManager.CreateBooking(new CreateBookingRequest { Data = second });

    Assert.That(secondRes.Success, Is.False);
    Assert.That(secondRes.ErrorCode, Is.EqualTo(ErrorCodes.CONFLICTING_BOOKING));
    Assert.That(_store.Count, Is.EqualTo(1));
}
```

---

### **7.3.5 Conclusão**

A suíte de testes do **BookingManager** garante:

- Validação completa das regras de negócio  
- Detecção correta de conflitos de datas  
- Tratamento adequado de exceções  
- Interação correta com múltiplos repositórios  
- Mapeamento correto entre DTOs e entidades  
- Comportamento previsível e robusto  

Com isso, a Feature Booking está totalmente validada e pronta para integração com o restante do sistema.


# 8 Feature Payment

---

## **8.1 Feature Payment**

### **8.1.1 Introdução**  
A Feature **Payment** adiciona ao sistema a capacidade de processar pagamentos externos utilizando provedores como:

- PayPal  
- Stripe  
- PagSeguro  
- MercadoPago  

Diferente das features anteriores, **Payment não pertence ao domínio do hotel**, mas sim à **camada Application**, pois:

- Não é uma regra de negócio do hotel  
- É uma integração externa  
- Pode mudar sem afetar o domínio  
- Pode ter múltiplos provedores plugáveis  

Por isso, a arquitetura segue o padrão:

- **DTOs** → Entrada e saída  
- **Enums** → Métodos e provedores suportados  
- **PaymentProcessor** → Interface para captura de pagamento  
- **PaymentProcessorFactory** → Criação dinâmica do processador correto  
- **PaymentResponse** → Resposta padronizada  

Essa estrutura permite adicionar novos provedores sem alterar o restante do sistema.

---

### **8.1.2 DTOs da Camada Application**

#### **8.1.2.1 PaymentRequestDTO**

Representa a intenção de pagamento enviada pelo cliente.

```csharp
public class PaymentRequestDTO
{
    public int BookingId { get; set; }

    public string PaymentIntention { get; set; } = null!;

    public SupportedPaymentProviders SelectedPaymentProvider { get; set; }
    public SupportedPaymentMethods SelectedPaymentMethod { get; set; }
}
```

---

#### **8.1.2.2 PaymentStateDTO**

Representa o estado final do pagamento após a tentativa de captura.

```csharp
public class PaymentStateDTO
{
    public Status Status  { get; set; }

    public string PaymentId { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public string Message  { get; set; } = null!;
}
```

---

### **8.1.3 Enums da Feature Payment**

#### **8.1.3.1 Status**

```csharp
public enum Status
{
    Success,
    Failed,
    Error,
    Undefined
}
```

---

#### **8.1.3.2 SupportedPaymentMethods**

```csharp
public enum SupportedPaymentMethods
{
    DebitCard = 1,
    CreditCard = 2,
    BankTransfer = 3
}
```

---

#### **8.1.3.3 SupportedPaymentProviders**

```csharp
public enum SupportedPaymentProviders
{
    PayPal = 1,
    Stripe = 2,
    PagSeguro = 3,
    MercadoPago = 4
}
```

---

### **8.1.4 Interfaces da Camada Application**

#### **8.1.4.1 IPaymentProcessor**

Define o contrato para qualquer processador de pagamento.

```csharp
public interface IPaymentProcessor
{
    Task<PaymentResponse> CapturePayment(string paymentIntention);
}
```

Cada provedor (PayPal, Stripe, etc.) implementará essa interface.

---

#### **8.1.4.2 IPaymentProcessorFactory**

Define o contrato para criação dinâmica do processador correto.

```csharp
public interface IPaymentProcessorFactory
{
    IPaymentProcessor GetPaymentProcessor(SupportedPaymentProviders selectedPaymentProvider);
}
```

Essa factory permite adicionar novos provedores sem alterar o código existente.

---

### **8.1.5 PaymentResponse**

A resposta padronizada para qualquer operação de pagamento.

```csharp
public class PaymentResponse : Response
{
    public PaymentStateDTO Data { get; set; } = null!;
}
```

---

### **8.1.6 Conclusão**

A Feature Payment estabelece a base para integração com provedores externos de pagamento.  
A arquitetura foi projetada para ser:

- **Extensível** → novos provedores podem ser adicionados facilmente  
- **Isolada** → não afeta o domínio  
- **Testável** → processadores podem ser mockados  
- **Flexível** → suporta múltiplos métodos e provedores  
---

## **8.2 BookingManager e BookingController (Integração com Pagamentos)**

### **8.2.1 Introdução**  
Nesta etapa, evoluímos a Feature Booking para suportar **pagamentos externos**, integrando a camada Application com a Feature Payment criada na seção 8.1.

A partir de agora, o fluxo de reserva passa a ter duas etapas:

1. **Criação da reserva**  
2. **Pagamento da reserva**

Para isso, adicionamos ao `BookingManager`:

- Um novo método: **PayForABooking**
- Injeção da **IPaymentProcessorFactory**
- Encaminhamento da intenção de pagamento para o provedor correto

E ao `BookingController`:

- Um novo endpoint:  
  `POST /booking/{bookingId}/pay`

Essa integração segue rigorosamente os princípios de:

- DDD  
- Arquitetura Hexagonal  
- Clean Architecture  
- SOLID  

---

### **8.2.2 Atualização do BookingManager**

#### **8.2.2.1 Construtor atualizado**

```csharp
public BookingManager(
    IBookingRepository bookingRepository,
    IGuestRepository guestRepository,
    IRoomRepository roomRepository,
    IMapper mapper,
    IPaymentProcessorFactory paymentProcessorFactory)
{
    _bookingRepository = bookingRepository;
    _guestRepository = guestRepository;
    _roomRepository = roomRepository;
    _mapper = mapper;
    _paymentProcessorFactory = paymentProcessorFactory;
}
```

---

#### **8.2.2.2 Método PayForABooking**

```csharp
public async Task<PaymentResponse> PayForABooking(PaymentRequestDTO paymentRequestDTO)
{
    var paymentProcessor = _paymentProcessorFactory
        .GetPaymentProcessor(paymentRequestDTO.SelectedPaymentProvider);

    var response = await paymentProcessor.CapturePayment(paymentRequestDTO.PaymentIntention);

    if (response.Success)
    {
        return new PaymentResponse
        {
            Success = true,
            Data = response.Data,
            Message = "Payment successfully processed"
        };
    }

    return response;
}
```

---

### **8.2.3 BookingController Atualizado**

#### **8.2.3.1 Endpoint de criação de reserva**

```csharp
[HttpPost]
public async Task<ActionResult<ReturnBookingDTO>> Post(CreateBookingDTO booking)
{
    var request = new CreateBookingRequest { Data = booking };
        
    var res = await _bookingManager.CreateBooking(request);

    if(res.Success) return Created("", res.Data);

    if(
        res.ErrorCode == ErrorCodes.INVALID_DATES || 
        res.ErrorCode == ErrorCodes.MISSING_REQUIRED_INFORMATION_BOOKING || 
        res.ErrorCode == ErrorCodes.INVALID_GUEST_ID ||
        res.ErrorCode == ErrorCodes.INVALID_ROOM_ID ||
        res.ErrorCode == ErrorCodes.BOOKING_COULD_NOT_BE_CREATED ||
        res.ErrorCode == ErrorCodes.NOT_FOUND
    )
    {
        return BadRequest(res);
    }

    _logger.LogError("Response with unknown ErrorCode Returned{@res}", res);
    return StatusCode(StatusCodes.Status500InternalServerError, res);
}
```

---

#### **8.2.3.2 Novo endpoint: PayForABooking**

```csharp
[HttpPost]
[Route("{bookingId}/Pay")]
public async Task<ActionResult<PaymentResponse>> Pay(
    PaymentRequestDTO paymentRequestDTO, int bookingId
)
{
    paymentRequestDTO.BookingId = bookingId;

    var res = await _bookingManager.PayForABooking(paymentRequestDTO);

    if (res.Success) return Ok(res.Data);

    return BadRequest(res);
}
```

---

#### **8.2.3.3 Endpoint de consulta de reserva**

```csharp
[HttpGet]
public async Task<ActionResult<ReturnBookingDTO>> Get(int bookingId)
{
    var res = await _bookingManager.GetBooking(bookingId);

    if(res.Success) return Ok(res.Data);

    if(res.ErrorCode == ErrorCodes.NOT_FOUND)
    {
        return NotFound(res);
    }

    _logger.LogError("Response with unknown ErrorCode Returned{@res}", res);
    return StatusCode(StatusCodes.Status500InternalServerError, res);
}
```

---

### **8.2.4 Fluxo Completo de Pagamento**

1. Cliente cria uma reserva  
2. Cliente inicia o pagamento  
3. BookingManager seleciona o provedor correto  
4. PaymentProcessor executa a captura  
5. PaymentResponse retorna o estado final  

---

### **8.2.5 Benefícios Arquiteturais**

- Extensível  
- Baixo acoplamento  
- Testável  
- Segue DIP  
- Permite múltiplos provedores  

---

### **8.2.6 Conclusão**

Com a integração entre Booking e Payment:

- O sistema agora suporta pagamentos reais  
- A arquitetura continua limpa e modular  
- A lógica de negócio permanece isolada  
- A API expõe endpoints claros e consistentes  

---

## **8.3 Microserviço Payment**

### **8.3.1 Introdução**  
A Feature Payment agora evolui para um **microserviço de pagamento**, responsável por:

- Processar pagamentos externos  
- Integrar provedores reais (ex.: MercadoPago)  
- Permitir fallback para provedores não implementados  
- Retornar estados padronizados de pagamento  
- Ser plugável e extensível  

Nesta etapa, implementamos:

- **MercadoPagoAdapter**  
- **NotImplementedPaymentProvider**  
- **PaymentProcessorFactory**  
- **Exceções específicas**  

Esse design segue o padrão **Adapter + Factory**, garantindo baixo acoplamento e alta extensibilidade.

---

### **8.3.2 MercadoPagoAdapter**

O adapter simula a integração com o MercadoPago.  
Ele implementa `IPaymentProcessor` e encapsula toda a lógica específica do provedor.

#### **8.3.2.1 Implementação**

```csharp
public class MercadoPagoAdapter : IPaymentProcessor
{
    public Task<PaymentResponse> CapturePayment(string paymentIntention)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(paymentIntention))
            {
                throw new InvalidaPaymentIntentionException();
            }

            paymentIntention += "/success";

            var dto = new PaymentStateDTO
            {
                CreatedDate = DateTime.Now,
                Message = $"Successfully paid {paymentIntention}",
                PaymentId = "123",
                Status = Payment.Enums.Status.Success
            };

            var resp = new PaymentResponse
            {
                Success = true,
                Data = dto,
                Message = "Payment sucessfully processed"
            };

            return Task.FromResult(resp);
        }
        catch (InvalidaPaymentIntentionException)
        {
            var resp = new PaymentResponse
            {
                Success = false,
                ErrorCode = ErrorCodes.INVALID_PAYMENT_INTENTION,
            };

            return Task.FromResult(resp);
        }
    }
}
```

---

### **8.3.3 Exceções do MercadoPago**

```csharp
public class InvalidaPaymentIntentionException : Exception
{
}
```

---

### **8.3.4 NotImplementedPaymentProvider**

Esse provider é retornado quando o usuário seleciona um provedor ainda não implementado.

#### **8.3.4.1 Implementação**

```csharp
public class NotImplementedPaymentProvider : IPaymentProcessor
{
    public Task<PaymentResponse> CapturePayment(string paymentIntention)
    {
        var paymentResponse = new PaymentResponse
        {
            Success = false,
            ErrorCode = ErrorCodes.PAYMENT_PROVIDER_NOT_IMPLEMENTED,
            Message = "The selected payment provider iis not available at the moment"
        };

        return Task.FromResult(paymentResponse);
    }
}
```

---

### **8.3.5 PaymentProcessorFactory**

A factory é responsável por retornar o processador correto com base no provedor selecionado.

#### **8.3.5.1 Implementação**

```csharp
public class PaymentProcessorFactory : IPaymentProcessorFactory
{
    public IPaymentProcessor GetPaymentProcessor(SupportedPaymentProviders selectedPaymentProvider)
    {
        switch(selectedPaymentProvider)
        {
            case SupportedPaymentProviders.MercadoPago:
                return new MercadoPagoAdapter();

            default:
                return new NotImplementedPaymentProvider();
        }
    }
}
```

---

### **8.3.6 Fluxo Completo do Microserviço Payment**

1. O cliente envia um `PaymentRequestDTO`  
2. O BookingManager chama a **PaymentProcessorFactory**  
3. A factory retorna o adapter correto  
4. O adapter processa o pagamento  
5. O resultado é retornado como **PaymentResponse**  
6. O BookingController retorna o estado final ao cliente  

---

### **8.3.7 Benefícios Arquiteturais**

- **Plugável**: novos provedores podem ser adicionados sem alterar o BookingManager  
- **Isolado**: lógica de pagamento não contamina o domínio  
- **Testável**: adapters podem ser mockados facilmente  
- **Extensível**: basta criar novos adapters e registrar na factory  
- **Seguro**: erros são tratados e convertidos em `PaymentResponse`  

---

### **8.3.8 Conclusão**

Com o microserviço Payment implementado:

- O sistema agora suporta múltiplos provedores  
- A arquitetura permanece limpa e modular  
- O fluxo de pagamento está totalmente integrado ao Booking  
- A factory permite expansão futura sem impacto no restante do sistema  
---

## **8.4 Testando o Microserviço Payment**

### **8.4.1 Introdução**  
A Feature Payment foi projetada para ser totalmente plugável e desacoplada, permitindo que diferentes provedores de pagamento sejam adicionados sem alterar o restante do sistema.  
Para garantir sua confiabilidade, criamos testes unitários que validam:

- O comportamento do **MercadoPagoAdapter**
- O comportamento da **PaymentProcessorFactory**
- O fallback para provedores não implementados
- O tratamento de erros (ex.: Payment Intention inválida)

Os testes utilizam:

- **NUnit**
- **Instâncias reais da factory**
- **Execução assíncrona dos adapters**

---

### **8.4.2 Testes do MercadoPagoAdapter**

#### **8.4.2.1 Testes Positivos**

```csharp
[Test]
public void Should_Return_Mercado_Pago_Adapter_Provider()
{
    var factory = new PaymentProcessorFactory();

    var provider = factory.GetPaymentProcessor(SupportedPaymentProviders.MercadoPago);

    Assert.That(provider, Is.TypeOf<MercadoPagoAdapter>());
}

[Test]
public async Task ShoudSucessfullyProcessPaymentAsync()
{
    var factory = new PaymentProcessorFactory();

    var provider = factory.GetPaymentProcessor(SupportedPaymentProviders.MercadoPago);

    var res = await provider.CapturePayment($"https://www.mercadopago.com/asdf");

    Assert.That(res.Success, Is.True);
    Assert.That(res.Data, Is.Not.Null);
    Assert.That(res.Data.CreatedDate, Is.Not.EqualTo(default(DateTime)));
    Assert.That(res.Data.PaymentId, Is.Not.Null);
}
```

Esses testes garantem que:

- O provider correto é retornado pela factory  
- O pagamento é processado com sucesso  
- O DTO retornado contém dados válidos  

---

#### **8.4.2.2 Testes Negativos**

```csharp
[Test]
public async Task Should_Fail_When_Payment_Intention_String_Is_InvalidAsync()
{
    var factory = new PaymentProcessorFactory();

    var provider = factory.GetPaymentProcessor(SupportedPaymentProviders.MercadoPago);

    var res = await provider.CapturePayment("");

    Assert.That(res.Success, Is.False);
    Assert.That(res.ErrorCode, Is.EqualTo(ErrorCodes.INVALID_PAYMENT_INTENTION));
}
```

Esse teste valida:

- O tratamento correto da exceção `InvalidaPaymentIntentionException`
- O retorno do `ErrorCodes.INVALID_PAYMENT_INTENTION`

---

### **8.4.3 Testes da PaymentProcessorFactory**

#### **8.4.3.1 Testes Positivos**

```csharp
[Test]
public void Should_Return_Mercado_Pago_Provider()
{
    var factory = new PaymentProcessorFactory();

    var provider = factory.GetPaymentProcessor(SupportedPaymentProviders.MercadoPago);

    Assert.That(provider, Is.TypeOf<MercadoPagoAdapter>());
}
```

Esse teste garante que a factory retorna o adapter correto quando o provedor é suportado.

---

#### **8.4.3.2 Testes Negativos**

```csharp
[TestCase(SupportedPaymentProviders.PagSeguro)]
[TestCase(SupportedPaymentProviders.PayPal)]
[TestCase(SupportedPaymentProviders.Stripe)]
public async Task Should_Return_Not_Implemented_Payment_Provider(SupportedPaymentProviders paymentProviders)
{
    var factory = new PaymentProcessorFactory();

    var provider = factory.GetPaymentProcessor(paymentProviders);

    Assert.That(provider, Is.TypeOf<NotImplementedPaymentProvider>());

    var res = await provider.CapturePayment($"https://www.{paymentProviders}.com/asdf");

    Assert.That(res.Success, Is.False);
    Assert.That(res.ErrorCode, Is.EqualTo(ErrorCodes.PAYMENT_PROVIDER_NOT_IMPLEMENTED));
}
```

Esse teste garante que:

- Provedores não implementados retornam `NotImplementedPaymentProvider`
- O erro correto é retornado (`PAYMENT_PROVIDER_NOT_IMPLEMENTED`)

---

### **8.4.4 Conclusão**

A suíte de testes do microserviço Payment garante:

- Funcionamento correto do MercadoPagoAdapter  
- Tratamento adequado de erros  
- Factory funcionando como ponto único de criação  
- Fallback seguro para provedores não implementados  
- Comportamento previsível e robusto  

Com isso, a Feature Payment está totalmente validada e pronta para integração com o restante do sistema.
---

## **8.5 Atualizando Status do Booking**

### **8.5.1 Introdução**  
Até este ponto, o sistema permitia:

- Criar reservas  
- Consultar reservas  
- Processar pagamentos  

Agora, evoluímos a Feature Booking para permitir **atualização do status da reserva**, garantindo que:

- Após o pagamento, a reserva muda de **Created → Paid**  
- Após finalização, muda de **Paid → Finished**  
- Após cancelamento, muda de **Created/Paid → Canceled**  
- Após reabertura, muda de **Canceled → Created**  
- Após reembolso, muda de **Paid → Refounded**

Essa lógica é **100% controlada pelo domínio**, através do método:

```
booking.ChangeState(Action action)
```

E persistida via:

```
await booking.Save(...)
```

Para isso, foram necessárias três atualizações:

1. **Adicionar Update ao IBookingRepository**  
2. **Atualizar o método Save do Booking**  
3. **Atualizar o BookingManager para persistir mudanças de status**  

---

### **8.5.2 Atualização do IBookingRepository**

#### **8.5.2.1 Novo método Update**

```csharp
public interface IBookingRepository
{
    Task<Entities.Booking?> Get(int id);

    Task<int> Create(Entities.Booking booking);

    Task<bool> ExistsActiveBookingForRoom(int roomId, DateTime start, DateTime end);

    Task Update(Entities.Booking booking);
}
```

Esse método permite persistir mudanças de status sem recriar a reserva.

---

### **8.5.3 Atualização da Entidade Booking**

#### **8.5.3.1 Save agora chama Update quando Id != 0**

```csharp
public async Task Save(
    IBookingRepository bookingRepository,
    IGuestRepository guestRepository,
    IRoomRepository roomRepository)
{
    await Validate(guestRepository, roomRepository, bookingRepository);
        
    if (Id == 0)
    {
        Id = await bookingRepository.Create(this);
    }
    else
    {
        await bookingRepository.Update(this);
    }
}
```

Agora o domínio controla:

- Quando criar  
- Quando atualizar  

---

### **8.5.4 Atualização do BookingManager**

O BookingManager agora:

- Valida se a reserva existe antes do pagamento  
- Processa o pagamento  
- Atualiza o status da reserva  
- Persiste a mudança via Save  
- Usa o novo **BookingExceptionMapper**  
- Usa o novo **ResponseFactory**  

#### **8.5.4.1 Método PayForABooking atualizado**

```csharp
public async Task<PaymentResponse> PayForABooking(PaymentRequestDTO paymentRequestDTO)
{
    try
    {
        var booking = await _bookingRepository.Get(paymentRequestDTO.BookingId);

        if (booking == null)
        {
            return new PaymentResponse
            {
                Success = false,
                ErrorCode = ErrorCodes.INVALID_BOOKING_ID,
                Message = "No booking found with the provided id"
            };
        }

        var paymentProcessor = _paymentProcessorFactory
            .GetPaymentProcessor(paymentRequestDTO.SelectedPaymentProvider);

        var response = await paymentProcessor.CapturePayment(paymentRequestDTO.PaymentIntention);

        if (!response.Success)
        {
            return response;
        }

        booking.ChangeState(Domain.Booking.Enums.Action.Pay);

        await booking.Save(_bookingRepository, _guestRepository, _roomRepository);

        return ResponseFactory.Ok<PaymentResponse>(r =>
        {
            r.Data = response.Data;
            r.Message = "Payment successfully processed";
        });
    }
    catch (Exception ex)
    {
        var failure = BookingExceptionMapper.Map(ex);

        return ResponseFactory.Fail<PaymentResponse>(failure);
    }
}
```

---

### **8.5.5 BookingExceptionMapper**

Agora todas as exceções do domínio são convertidas em:

- `ErrorCode`
- `Message`

#### **8.5.5.1 Implementação**

```csharp
public static class BookingExceptionMapper
{
    public static FailureInfo Map(Exception ex)
    {
        return ex switch
        {
            MissingRequiredInformation => new FailureInfo
            {
                ErrorCode = ErrorCodes.MISSING_REQUIRED_INFORMATION_BOOKING,
                Message = "Some required information for creating the booking was not provided"
            },

            InvalidBookingDatesException => new FailureInfo
            {
                ErrorCode = ErrorCodes.INVALID_DATES,
                Message = "The provided booking dates are invalid"
            },

            ConflictingBookingException => new FailureInfo
            {
                ErrorCode = ErrorCodes.CONFLICTING_BOOKING,
                Message = "The provided room is not available for the selected dates"
            },

            Domain.Guest.Exceptions.GuestExceptons => new FailureInfo
            {
                ErrorCode = ErrorCodes.INVALID_DATA_GUEST,
                Message = "The provided guest has invalid data"
            },

            Domain.Room.Exceptions.RoomExceptions => new FailureInfo
            {
                ErrorCode = ErrorCodes.INVALID_DATA_ROOM,
                Message = "The provided room has invalid data"
            },

            InvalidGuestIDException => new FailureInfo
            {
                ErrorCode = ErrorCodes.INVALID_GUEST_ID,
                Message = "The provided guest has invalid data"
            },

            InvalidRoomIDException => new FailureInfo
            {
                ErrorCode = ErrorCodes.INVALID_ROOM_ID,
                Message = "The provided room does not exist"
            },

            _ => new FailureInfo
            {
                ErrorCode = ErrorCodes.BOOKING_COULD_NOT_BE_CREATED,
                Message = "An unexpected error occurred"
            }
        };
    }
}
```

---

### **8.5.6 ResponseFactory**

Simplifica a criação de respostas:

#### **8.5.6.1 Implementação**

```csharp
public static class ResponseFactory
{
    public static T Fail<T>(FailureInfo failure)
        where T : Response, new()
    {
        return new T
        {
            Success = false,
            ErrorCode = failure.ErrorCode,
            Message = failure.Message
        };
    }

    public static T Ok<T>(Action<T> configure)
        where T : Response, new()
    {
        var response = new T { Success = true };
        configure(response);
        return response;
    }
}
```

---

### **8.5.7 Conclusão**

Com a atualização do status do Booking:

- O domínio agora controla transições de estado  
- O repositório suporta atualizações  
- O BookingManager integra pagamento + mudança de estado  
- O sistema está pronto para:
  - Cancelamento de reservas  
  - Finalização de estadias  
  - Reabertura de reservas  
  - Reembolsos  

A arquitetura continua:

- Limpa  
- Extensível  
- Segura  
- Testável  

# 9 Introduzindo CQRS com MediatR
## 9.1 CQRS Introdução

### 9.1.1 Visão Geral  
Command Query Responsibility Segregation (CQRS) é um padrão arquitetural que separa explicitamente operações de escrita (commands) das operações de leitura (queries). Essa separação permite otimizações independentes, maior clareza de responsabilidades e alinhamento com arquiteturas orientadas ao domínio e microserviços.

### 9.1.2 Motivação  
A unificação de leitura e escrita em um único modelo tende a gerar complexidade conforme o sistema cresce. CQRS propõe a divisão desses fluxos para:  
- Reduzir acoplamento entre operações.  
- Permitir modelos distintos para leitura e escrita.  
- Facilitar escalabilidade horizontal.  
- Melhorar performance em cenários de alta demanda.

### 9.1.3 Relação com DDD e Arquitetura Hexagonal  
CQRS se integra naturalmente ao DDD ao permitir que o modelo de escrita represente fielmente o domínio, enquanto o modelo de leitura pode ser otimizado para consultas.  
Na Arquitetura Hexagonal, commands e queries são tratados como portas de entrada, enquanto handlers e repositórios atuam como adaptadores.

### 9.1.4 Estrutura Conceitual  
#### 9.1.4.1 Commands  
Representam intenções de mudança de estado.  
- São imperativos.  
- Não retornam dados complexos.  
- São processados por handlers específicos.

#### 9.1.4.2 Queries  
Representam solicitações de leitura.  
- Não alteram estado.  
- São idempotentes.  
- Podem utilizar modelos de dados otimizados.

### 9.1.5 Exemplo de Estrutura CQRS em C#  
#### 9.1.5.1 Command  
```csharp
public record CriarClienteCommand(Guid ClienteId, string Nome) : IRequest<bool>;
```

#### 9.1.5.2 Command Handler  
```csharp
public class CriarClienteHandler : IRequestHandler<CriarClienteCommand, bool>
{
    private readonly IClienteRepository _repository;

    public CriarClienteHandler(IClienteRepository repository)
    {
        _repository = repository;
    }

    public Task<bool> Handle(CriarClienteCommand request, CancellationToken cancellationToken)
    {
        var cliente = new Cliente(request.ClienteId, request.Nome);
        _repository.Salvar(cliente);
        return Task.FromResult(true);
    }
}
```

#### 9.1.5.3 Query  
```csharp
public record ObterClienteQuery(Guid ClienteId) : IRequest<ClienteDto>;
```

#### 9.1.5.4 Query Handler  
```csharp
public class ObterClienteHandler : IRequestHandler<ObterClienteQuery, ClienteDto>
{
    private readonly IClienteReadRepository _readRepository;

    public ObterClienteHandler(IClienteReadRepository readRepository)
    {
        _readRepository = readRepository;
    }

    public Task<ClienteDto> Handle(ObterClienteQuery request, CancellationToken cancellationToken)
    {
        return _readRepository.ObterPorIdAsync(request.ClienteId);
    }
}
```

### 9.1.6 Comparação entre Modelo Tradicional e CQRS  
| Critério | Modelo Tradicional | CQRS |
|---------|--------------------|------|
| Modelo de Dados | Único | Separado (leitura/escrita) |
| Escalabilidade | Limitada | Independente por operação |
| Complexidade | Centralizada | Distribuída e modular |
| Performance | Geral | Otimizada por fluxo |

### 9.1.7 Benefícios  
- Redução de acoplamento entre leitura e escrita.  
- Maior clareza de responsabilidades.  
- Possibilidade de usar tecnologias distintas para cada lado.  
- Facilita event sourcing, quando necessário.

### 9.1.8 Considerações de Uso  
CQRS não deve ser aplicado indiscriminadamente. É mais adequado quando:  
- Há grande volume de leitura.  
- O modelo de domínio é complexo.  
- Há necessidade de escalabilidade independente.  
- O sistema utiliza eventos de domínio ou event sourcing.

### 9.1.9 Exemplo Prático de Fluxo  
1. O cliente envia um comando CriarClienteCommand.  
2. O handler valida e cria o agregado Cliente.  
3. O repositório persiste o agregado.  
4. Um evento de domínio pode ser disparado.  
5. O lado de leitura atualiza sua projeção.  
6. Consultas subsequentes utilizam o modelo otimizado de leitura.

### 9.1.10 Conclusão  
CQRS fornece uma separação clara entre operações de leitura e escrita, permitindo que cada fluxo evolua de forma independente. Em conjunto com DDD, Hexagonal Architecture e microserviços, o padrão contribui para sistemas mais escaláveis, organizados e alinhados ao domínio.

## 9.2 Usando MediatR

### 9.2.1 Introdução  
MediatR é uma biblioteca que implementa o padrão Mediator, permitindo desacoplamento entre controladores, casos de uso e lógica de domínio. Em arquiteturas baseadas em CQRS, DDD e Hexagonal Architecture, MediatR atua como um ponto central para o envio de comandos e queries, eliminando dependências diretas entre camadas e facilitando testes, manutenção e extensibilidade.

### 9.2.2 Instalação do Pacote  
A instalação é realizada via CLI do .NET:

```bash
dotnet add package MediatR
```

Esse comando adiciona o pacote ao projeto, permitindo registrar handlers e enviar comandos e queries.

### 9.2.3 Registro do MediatR no Container de Injeção de Dependência  
O registro é feito no `Program.cs`, apontando para o assembly que contém os handlers:

```csharp
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblyContaining<BookingManager>();
});
```

Esse registro garante que todos os handlers localizados no assembly sejam automaticamente descobertos e registrados.

### 9.2.4 Estrutura Arquitetural com MediatR  
#### 9.2.4.1 Fluxo de Comando  
1. Controller recebe a requisição.  
2. Controller cria um comando.  
3. Controller envia o comando via `IMediator`.  
4. Handler processa o comando.  
5. Handler delega a lógica ao caso de uso (Application Service).  
6. Caso de uso interage com o domínio e repositórios.  
7. Handler retorna a resposta ao controller.

#### 9.2.4.2 Benefícios  
- Redução de acoplamento entre camadas.  
- Facilita testes unitários.  
- Permite evolução modular de comandos e queries.  
- Alinha-se ao padrão CQRS.

### 9.2.5 Exemplo Completo de Uso do MediatR  
A seguir, a estrutura apresentada no código fornecido é organizada e explicada tecnicamente.

#### 9.2.5.1 Controller utilizando MediatR  
```csharp
[HttpPost]
public async Task<ActionResult<ReturnBookingDTO>> Post(CreateBookingDTO booking)
{
    var request = new CreateBookingRequest { Data = booking };

    var command = new CreateBookingComand
    {
        createBookingRequest = request
    };

    var res = await _mediator.Send(command);

    if(res.Success) return Created("", res.Data);

    if(
        res.ErrorCode == ErrorCodes.INVALID_DATES || 
        res.ErrorCode == ErrorCodes.MISSING_REQUIRED_INFORMATION || 
        res.ErrorCode == ErrorCodes.INVALID_GUEST_ID ||
        res.ErrorCode == ErrorCodes.INVALID_ROOM_ID ||
        res.ErrorCode == ErrorCodes.COULD_NOT_STORE_DATA ||
        res.ErrorCode == ErrorCodes.NOT_FOUND
    )
    {
        return BadRequest(res);
    }

    _logger.LogError("Response with unknown ErrorCode Returned{@res}", res);
    return StatusCode(StatusCodes.Status500InternalServerError, res);
}
```

O controller não chama diretamente o caso de uso. Ele apenas cria o comando e o envia ao MediatR.

#### 9.2.5.2 Definição do Command  
```csharp
public class CreateBookingComand : IRequest<BookingResponse>
{
    public CreateBookingRequest createBookingRequest { get; set; } = null!;
}
```

O comando implementa `IRequest<T>`, indicando o tipo de retorno esperado.

#### 9.2.5.3 Handler do Command  
```csharp
public class CreateBookingComandHandler : IRequestHandler<CreateBookingComand, BookingResponse>
{
    private IBookingManager _bookingManager;

    public CreateBookingComandHandler(IBookingManager bookingManager)
    {
        _bookingManager = bookingManager;
    }

    public Task<BookingResponse> Handle(CreateBookingComand request, CancellationToken cancellationToken)
    {
        return _bookingManager.CreateBooking(request.createBookingRequest);
    }
}
```

O handler recebe o comando e delega a lógica ao caso de uso (`BookingManager`), mantendo o domínio isolado.

### 9.2.6 Integração com DDD e Hexagonal Architecture  
#### 9.2.6.1 Ports and Adapters  
- O handler atua como um adapter de entrada.  
- O caso de uso (`BookingManager`) é a porta de aplicação.  
- Repositórios são adapters de saída.  

Essa separação mantém o domínio independente de frameworks.

#### 9.2.6.2 Vantagens no Contexto DDD  
- Commands representam intenções do usuário.  
- Handlers orquestram casos de uso.  
- O domínio permanece puro e isolado.

### 9.2.7 Comparação: Controller Chamando Serviço vs Controller Usando MediatR  
| Critério | Sem MediatR | Com MediatR |
|---------|--------------|-------------|
| Acoplamento | Alto (controller → service) | Baixo (controller → mediator → handler) |
| Testabilidade | Moderada | Alta |
| Evolução | Difícil | Modular |
| CQRS | Limitado | Naturalmente suportado |

### 9.2.8 Boas Práticas ao Usar MediatR  
- Commands devem ser simples e representar intenções.  
- Handlers devem ser finos e delegar lógica ao caso de uso.  
- Evitar colocar lógica de domínio dentro de handlers.  
- Queries e commands devem ser separados.  
- Evitar handlers que retornam tipos complexos desnecessariamente.

### 9.2.9 Exemplo Prático de Fluxo Completo  
1. Usuário envia requisição POST para criar reserva.  
2. Controller cria `CreateBookingComand`.  
3. Controller envia comando ao MediatR.  
4. Handler recebe comando e chama `BookingManager`.  
5. `BookingManager` valida, cria entidade e salva via repositórios.  
6. Handler retorna `BookingResponse`.  
7. Controller retorna `201 Created`.

### 9.2.10 Conclusão  
O uso de MediatR em conjunto com CQRS, DDD e Hexagonal Architecture promove desacoplamento, modularidade e clareza arquitetural. A separação entre comandos, handlers e casos de uso facilita manutenção, testes e evolução do sistema, mantendo o domínio isolado e aderente aos princípios SOLID.


## 9.3 MediatR Query

### 9.3.1 Introdução  
Queries representam operações de leitura dentro do padrão CQRS. Diferentemente dos comandos, queries não alteram o estado do sistema e são utilizadas exclusivamente para recuperar informações. MediatR fornece um mecanismo simples e desacoplado para enviar queries e receber respostas, mantendo a arquitetura limpa e alinhada aos princípios de DDD e Hexagonal Architecture.

### 9.3.2 Estrutura de uma Query  
Uma query deve:  
- Representar uma intenção de leitura.  
- Ser imutável sempre que possível.  
- Implementar `IRequest<TResponse>`, onde `TResponse` é o tipo retornado.  
- Ser tratada por um handler específico.

### 9.3.3 Definição da Query  
A seguir, a query utilizada para recuperar uma reserva:

```csharp
public class GetBookingQuery : IRequest<BookingResponse>
{
    public int bookingId { get; set; }
}
```

Essa query encapsula apenas os dados necessários para a operação de leitura, mantendo simplicidade e clareza.

### 9.3.4 Implementação do Query Handler  
O handler é responsável por processar a query e retornar o resultado. Ele atua como um adapter de entrada na Arquitetura Hexagonal.

```csharp
public class GetBookingQueryHandler : IRequestHandler<GetBookingQuery, BookingResponse>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IMapper _mapper;

    public GetBookingQueryHandler(
        IBookingRepository bookingRepository,
        IMapper mapper
    )
    {
        _bookingRepository = bookingRepository;
        _mapper = mapper;
    }

    public async Task<BookingResponse> Handle(GetBookingQuery request, CancellationToken cancellationToken)
    {
        var guest = await _bookingRepository.Get(request.bookingId);

        if (guest == null)
        {
            return new BookingResponse
            {
                Success = false,
                ErrorCode = ErrorCodes.NOT_FOUND,
                Message = "No booking found with the provided id"
            };
        }

        return new BookingResponse
        {
            Data = _mapper.Map<ReturnBookingDTO>(guest),
            Success = true,
        };
    }
}
```

### 9.3.5 Fluxo Arquitetural da Query  
#### 9.3.5.1 Etapas do Processo  
1. O controller recebe a requisição HTTP GET.  
2. O controller cria uma instância de `GetBookingQuery`.  
3. O controller envia a query ao MediatR via `_mediator.Send(query)`.  
4. MediatR identifica o handler correspondente.  
5. O handler consulta o repositório.  
6. O handler retorna um `BookingResponse`.  
7. O controller retorna o resultado ao cliente.

### 9.3.6 Uso da Query no Controller  
O controller utiliza MediatR para enviar a query, mantendo baixo acoplamento:

```csharp
[HttpGet]
public async Task<ActionResult<ReturnBookingDTO>> Get(int bookingId)
{
    var query = new GetBookingQuery
    {
        bookingId = bookingId
    };

    var res = await _mediator.Send(query);

    if(res.Success) return Ok(res.Data);

    if(res.ErrorCode == ErrorCodes.NOT_FOUND)
    {
        return NotFound(res);
    }

    _logger.LogError("Response with unknown ErrorCode Returned{@res}", res);
    return StatusCode(StatusCodes.Status500InternalServerError, res);
}
```

### 9.3.7 Comparação entre Query e Command  
| Aspecto | Command | Query |
|---------|---------|--------|
| Finalidade | Alterar estado | Ler estado |
| Idempotência | Não garantida | Garantida |
| Retorno | Simples (bool, id) | Dados completos |
| Handler | Executa lógica de negócio | Executa leitura otimizada |

### 9.3.8 Boas Práticas para Queries  
- Queries devem ser simples e diretas.  
- Não incluir lógica de domínio no handler.  
- Utilizar DTOs específicos para leitura.  
- Evitar dependências desnecessárias no handler.  
- Garantir que o repositório de leitura seja otimizado para consultas.

### 9.3.9 Exemplo Prático de Fluxo Completo  
1. Cliente chama `GET /booking?bookingId=10`.  
2. Controller cria `GetBookingQuery`.  
3. Controller envia a query ao MediatR.  
4. Handler consulta o repositório.  
5. Handler mapeia entidade para DTO.  
6. Handler retorna `BookingResponse`.  
7. Controller retorna `200 OK` com o DTO.

### 9.3.10 Conclusão  
A implementação de queries com MediatR reforça a separação entre leitura e escrita, característica essencial do CQRS. O uso de handlers dedicados, repositórios específicos e DTOs de leitura mantém o sistema modular, testável e alinhado aos princípios de DDD e Hexagonal Architecture.
