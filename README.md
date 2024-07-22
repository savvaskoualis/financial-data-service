# Financial Data Service

## Overview

This solution provides a service to handle live financial instrument prices sourced from a public data provider.

## Components

The solution consists of the following projects:

1. **FinancialDataService.Api**: Provides REST API endpoints for getting available instruments and the latest price of a specific instrument.
2. **FinancialDataService.Jobs**: Fetches available instruments and price updates from an external data provider (Binance), updates the data store and publishes updates.
3. **FinancialDataService.Streams**: Exposes the websocket responsible for delivering price updates to subscribed users. 
4. **FinancialDataService.TcpServer**: Manages the TCP server for the simple TCP/IP backplane to distribute price updates. Used during development, replaced by redis
5. **FinancialDataService.DemoUI**: A Blazor application for demonstrating the solution, including API calls and WebSocket subscriptions.

## Improvements

- **Service Resilience**:
    - Implement retry policies and circuit breakers to handle transient failures and ensure service resilience.
    - Add health checks and monitoring to detect and recover from failures.

- **Testing**:
    - Develop comprehensive unit and integration tests to ensure the correctness and reliability of the application.
    - Use automated testing tools to validate functionality during the CI/CD pipeline.

- **Client-Side UI**:
    - In a real-world scenario, the UI would be implemented on the client side (e.g., using Blazor WebAssembly or another SPA framework) to reduce server load and improve performance.
    - This would involve subscribing to SignalR from the client-side application rather than from a Blazor Server app.

- **Scalability**:
    - To be fully scalable, this solution should be deployed to an orchestration cluster such as Kubernetes, which would provide automated scaling, load balancing, and self-healing capabilities.

## Architecture

### System Components Diagram

![System Components](http://www.plantuml.com/plantuml/proxy?cache=no&src=https://raw.githubusercontent.com/savvaskoualis/financial-data-service/main/docs/SystemComponents.iuml)

### Scalability Design

- **Data Fetching and Storage**:
    - The `FinancialDataService.Jobs` component fetches available instruments and price updates from the external data provider, maintaining a single connection.
    - Maintaining a single connection is crucial for responsible scaling as the external provider may not allow multiple subscriptions.
    - Fetched data is stored in the database for efficient retrieval.

- **Serving API Requests**:
    - The `FinancialDataService.Api` component handles user requests for available instruments and the latest prices by querying the database.

- **Publishing Updates**:
    - In addition to updating the database, `FinancialDataService.Jobs` publishes price updates to the backplane (tcp/redis).

- **Distributing Updates**:
    - The `FinancialDataService.Streams` component subscribes to the backplane. Upon receiving updates, it pushes them to SignalR.
    - SignalR then distributes these updates to all connected WebSocket clients, ensuring real-time delivery to the end user.

- **Future Scalability with Distributed Backplane**:
    - For production, a clustered distributed backplane like Redis or Kafka should be used to handle thousands of subscribers efficiently.

This workflow ensures efficient data management and real-time update delivery, making the system scalable and capable of handling a large number of subscribers.

### Prerequisites

- .NET 8 SDK
- Visual Studio Code & docker plugin
- Docker and Docker Compose

### Setup

1. Clone the repository.
   ```sh
   git clone https://github.com/savvaskoualis/financial-data-service.git

2. Navigate to the project directory.
    ```sh
    cd financial-data-service

3. Open the project in Visual Studio Code.
    ```sh
    code .
   
4. Use the Docker extension in Visual Studio Code to build and run the services.
   You can use the command palette (Cmd+Shift+P on macOS, Ctrl+Shift+P on Windows/Linux) and type Docker: Compose Up to start all services.

5. Build and run the Docker Compose services. Open the Command Palette again and select Docker: Compose Up.

6. Alternatively, you can run the Docker Compose services using the terminal:
    ```sh
    docker compose up --build -d
   
7. To stop the services, use the Command Palette and select Docker: Compose Down or run the following command in the terminal:
    ```sh
    docker compose down

### Accessing the Application

- **API Swagger**: [http://localhost:61001/](http://localhost:61001/)
- **Demo UI**: [http://localhost:61004](http://localhost:61004)


## Docker Compose Configuration

The Docker Compose file includes the following services:

1. **postgres**: PostgreSQL database to store instrument and price data.
2. **redis**: Redis server to act as the distributed backplane.
3. **api**: REST API service to handle user requests.
4. **jobs**: Service to fetch instruments and price updates, and publish them to the backplane.
5. **streams**: WebSocket server to distribute price updates to subscribers.
6. **demoapp**: Blazor application for demonstrating the solution.

