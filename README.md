# Financial Data Service

## Overview

This solution provides a service to handle live financial instrument prices sourced from a public data provider. It includes both REST API and WebSocket endpoints, efficiently managing over 1,000 subscribers.

## Components

The solution consists of the following projects:

1. **FinancialDataService.Api**: Provides REST API endpoints for getting available instruments and the latest price of a specific instrument.
2. **FinancialDataService.Jobs**: Fetches available instruments and price updates from an external data provider, updates the data store and publishes updates.
3. **FinancialDataService.Streams**: Exposes the websocket responsible for delivering price updates to users.  Receives data from FinancialDataService.TcpServer.
4. **FinancialDataService.TcpServer**: Manages the TCP server for the simple TCP/IP backplane to distribute price updates.
5. **FinancialDataService.DemoUI**: A Blazor application for demonstrating the solution, including API calls and WebSocket subscriptions.


## Architecture

### System Components Diagram

![System Components](http://www.plantuml.com/plantuml/proxy?cache=no&src=https://raw.githubusercontent.com/savvaskoualis/financial-data-service/main/SystemComponents.iuml)
