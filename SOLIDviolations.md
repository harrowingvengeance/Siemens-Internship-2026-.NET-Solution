Violation 1:

What principle it violates: Single Responsibility 

File: Controllers/ItemController.cs, all action methods

Why it is a violation:  
The controller was doing more than routing HTTP requests. It contained business logic (computing statistics like TotalCount and AverageValue) and used Console.WriteLine directly for logging. A controller's single responsibility is to translate HTTP input into HTTP output. Business logic belongs in a service layer; logging belongs in ILogger<T>, not raw console calls mixed into action methods.

Fixes applied:  
All business logic moved to Services/ItemService.cs.
All Console.WriteLine calls replaced with injected ILogger<ItemController>, using structured logging (LogInformation, LogWarning).



Violation 2:  

What principle it violates: Single Responsibility

File: Repositories/ItemRepository.cs, methods GetByIdAsync and GetAllAsync

Why it is a violation:  
The repository was filtering items by IsActive. That is business logic, the decision of what counts as a "valid" or "visible" item belongs in the service layer. A repository's only responsibility is data access: fetch and return data as-is from the source. Mixing domain rules into the repository means the same rule lives in two places if the service ever also needs to reason about IsActive.

Fixes applied:  
The repository now returns raw data without any IsActive filtering. All IsActive checks are consolidated in ItemService, which is the single authoritative place for that business rule.



Violation 3: 

What principle it violates: Open/Closed

File: Repositories/ItemRepository.cs

Why it is a violation:  
The repository hard-coded its data source as an in-memory List<Item>. To switch to any external source (a database, an HTTP endpoint, a file), you would have to modify the existing class rather than extend it. Fields _items and _nextId were also protected, suggesting the original design anticipated inheritance as the extension mechanism, but inheritance for data-source swapping is fragile and couples subclasses to internal state. It is worth mentioning that ItemRepository already implemented IItemReader, meaning a new implementation could have been provided without modifying the class. The more precise problem is that the class had no real data source and the in-memory list was hard-coded with no way to swap it without touching the class internals.

Fixes applied: 
The repository was rewritten to accept an HttpClient via constructor injection and fetch data from the external endpoint. Switching data sources in future only requires providing a different IItemReader implementation, the class itself need not change.



Violation 4:

What principle it violates: Liskov Substitution

File: Repositories/ItemRepository.cs, methods GetByIdAsync and GetAllAsync (declared virtual)

Why it is a violation: 
Both public methods were marked virtual without any documented contract for what a subclass must guarantee. Combined with protected mutable fields (_items, _nextId), subclasses could override these methods in ways that break the invariants callers rely on (for example, returning inactive items, or items not present in _items). This is a potential Liskov Substition Principle hazard, the design risked substitution violations.

Fixes applied: 
The virtual/protected pattern was removed entirely. The class is no longer designed for inheritance. Extension is achieved through the IItemReader interface instead (composition over inheritance).



Violation 5:

What principle it violates: Interface Segregation

File: Interfaces/IItemReader.cs

Why it is a violation:  
IItemReader was the only interface, and it was injected directly into the controller. The controller used it for both read operations but there was no interface for the service itself, forcing the controller to reuse the repository interface, which is not appropriate. Clients should depend on interfaces tailored to their needs.

Fixes applied:  
A new IItemService interface was added in Interfaces/IItemService.cs. The controller now depends only on IItemService, which expresses exactly what the controller needs. The repository continues to implement IItemReader. Neither interface forces its consumers to depend on methods they do not use.



Violation 6:

What principle it violates: Dependency Inversion

File: Program.cs

Why it is a violation:  
The original Program.cs registered no services at all, no Dependency Injection registration for IItemReader  or ItemRepository was present, meaning the application would throw a runtime exception the moment the controller tried to resolve IItemReader. High-level modules depended on abstractions in name only; the composition root was broken.

Fixes applied:  
Program.cs now correctly registers:
- IItemReader → ItemRepository via AddHttpClient<IItemReader, ItemRepository>() (also wires the HttpClient dependency).
- IItemService → ItemService via AddScoped<IItemService, ItemService>().
High-level modules (ItemController, ItemService) depend only on abstractions. Low-level modules (ItemRepository) are injected at the composition root.

