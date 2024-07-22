# Financial Data Service

## Overview

This solution provides a service to handle live financial instrument prices sourced from a public data provider.

## Components

The solution consists of the following projects:

1. **FinancialDataService.Api**: Provides REST API endpoints for getting available instruments and the latest price of a specific instrument.
2. **FinancialDataService.Jobs**: Fetches available instruments and price updates from an external data provider, updates the data store and publishes updates.
3. **FinancialDataService.Streams**: Exposes the websocket responsible for delivering price updates to users.  Receives data from FinancialDataService.TcpServer.
4. **FinancialDataService.TcpServer**: Manages the TCP server for the simple TCP/IP backplane to distribute price updates.
5. **FinancialDataService.DemoUI**: A Blazor application for demonstrating the solution, including API calls and WebSocket subscriptions.


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
    - In addition to updating the database, `FinancialDataService.Jobs` publishes price updates to the backplane (currently TCP for demonstration purposes).

- **Distributing Updates**:
    - The `FinancialDataService.Streams` component subscribes to the backplane. Upon receiving updates, it pushes them to SignalR.
    - SignalR then distributes these updates to all connected WebSocket clients, ensuring real-time delivery to the end user.

- **Future Scalability with Distributed Backplane**:
    - For production, a distributed backplane like Redis or Kafka should be used to handle thousands of subscribers efficiently.
    - This approach ensures better load balancing, fault tolerance, and scalability, reliably delivering updates in a highly distributed environment.

This workflow ensures efficient data management and real-time update delivery, making the system scalable and capable of handling a large number of subscribers.

### Prerequisites

- .NET 8 SDK
- Visual Studio Code

### Setup

1. Clone the repository.
   ```sh
   git clone https://github.com/savvaskoualis/financial-data-service.git

2. Navigate to the project directory.
    ```sh
    cd financial-data-service

2. Navigate to the project directory.
    ```sh
    Start the projects by selecting the "Run All Projects" configuration from the Debug panel in Visual Studio Code and clicking the "Start Debugging" button