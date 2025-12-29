# Server-Sent Events (SSE) PoC .NET10 + Redis

This repository is a proof of concept (PoC) demonstrating how to implement
**Server-Sent Events (SSE)** in **.NET API** for real-time streaming
of newly created orders.

The PoC intentionally includes **two implementations** to highlight trade-offs:

1. **In-memory streaming** using `Channel<T>`
2. **Redis-backed streaming** using Redis Streams


## Goals of this PoC

- Demonstrate modern SSE support in .NET API
- Show clean DI-based design
- Support multiple concurrent SSE clients
- Compare in-memory vs Redis-based approaches
- Highlight real-world edge cases (fan-out, race conditions, retention)


## Implementation using In-Memory (Channel)

### Description
- Uses `System.Threading.Channels`
- Single shared channel registered as a **Singleton**
- Natural fan-out to all connected SSE clients

### When to use
- Local development
- Single-instance deployments
- Demos and experiments

### Characteristics
- **Fan-out:** Yes  
- **Replay:** No  
- **Multi-instance support:** No  
- **Persistence:** No  
- **Production-ready:** No  



## Implementation using Redis Streams

### Description
- Uses **Redis Streams** via `StackExchange.Redis`
- Each SSE connection maintains its own cursor
- Light polling is used (blocking reads are not supported)
- A **replay-window pattern** avoids race conditions

### When to use
- Multiple instances / Kubernetes
- Real users
- Shared event source across pods

### Characteristics
- **Fan-out:** Yes  
- **Replay:** Bounded (via replay window)  
- **Multi-instance support:** Yes  
- **Persistence:** Yes (bounded)  
- **Production-ready:** Yes  



## Replay Window Pattern

Redis Streams combined with polling can miss the **first event** when multiple
clients connect simultaneously.

To avoid this, the PoC uses a **replay window**


## High-Level Architecture
 
- create order  
- publish `OrderCreatedEvent`  
- SSE clients receive event via `/api/order/stream`


```mermaid
flowchart LR
    Client1[Browser / Client A]
    Client2[Browser / Client B]

    Client1 -->|SSE| StreamEndpoint
    Client2 -->|SSE| StreamEndpoint

    StreamEndpoint["GET /api/order/stream<br/>SSE Endpoint"]

    CreateOrder["POST /api/order<br/>Create Order API"]

    CreateOrder --> OrderService[Order Service]

    OrderService --> EventStream[IOrderEventStream]

    EventStream -->|In-Memory| InMemory[In-Memory Channel]
    EventStream -->|Redis| Redis[(Redis Streams)]

    InMemory --> StreamEndpoint
    Redis --> StreamEndpoint



