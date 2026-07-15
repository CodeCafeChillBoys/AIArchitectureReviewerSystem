# Microservices Patterns

- **Database per service**: Each microservice must have its own private database to ensure loose coupling.
- **API Gateway**: Use an API Gateway as a single entry point for all clients.
- **Circuit Breaker**: Implement circuit breakers to prevent cascading failures across services.
- **Saga Pattern**: Manage distributed transactions across multiple services using the Saga pattern.
