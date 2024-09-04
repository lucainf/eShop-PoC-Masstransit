# eShop PoC using Masstransit

A simple Proof of Concept (PoC) for learning MassTransit.

## Run

At the time of writing, everything should work except the API Gateway. To run the project:

1. Start a Docker container with RabbitMQ[^1].
2. Start each project inside the App namespace.

[^1]: i.e. `docker run -p 15672:15672 -p 5672:5672 masstransit/rabbitmq`

## TODOs:

- [ ] Models refactor

- [ ] APIGateway writing

- [ ] Docker-compose integration
