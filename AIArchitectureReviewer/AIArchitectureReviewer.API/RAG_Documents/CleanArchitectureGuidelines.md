# Clean Architecture Guidelines

1. **Dependency Rule**: Source code dependencies must only point inward, toward higher-level policies.
2. **Entities**: Encapsulate enterprise-wide business rules. An entity can be an object with methods, or it can be a set of data structures and functions.
3. **Use Cases**: Encapsulate and implement all of the use cases of the system.
4. **Interface Adapters**: Convert data from the format most convenient for the use cases and entities, to the format most convenient for some external agency such as the Database or the Web.
5. **Frameworks and Drivers**: Generally, you don't write much code in this layer other than glue code that communicates to the next circle inwards.
