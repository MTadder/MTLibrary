![](icon.png "MTLibrary C#")
# C# MTLibrary *v0.5.27.21*

- [x] DictionaryFile
  - `void` Set(`String` key, [`String` value = `String.Empty`]) 
  - `String` Get(`String` key) 
  - `void` Remove(`String` key)
  - `Boolean` IsValue(`String` value)
  - `Boolean` IsKey(`String` key)
  - `void` Load()
  - `void` Clear()
  - `void` Save()
- Networking
  - [ ] Redo this section with `Async` awareness. 
  - Server
    - [ ] `constructor` (`delegate void`(`Socket` newConnection))
    - [ ] `void` Send(`Socket` s, `Transmission` t)
    - [ ] `void` Send(`Socket` s, `Byte[]` data)
  - Client
  - Transmission
    - [x] `constructor` (`Socket` listenTo)
    - [x] `constructor` (`Byte[]` data)
    - [ ] Set(`String` key, `String` value)   
    - [x] Send(`Socket` to)
- Executor
  - [ ] `constructor`(`String` program, `String[]` arguments)
  - [ ] Execute()
- Display
  - [x] Write(`String` text, [`ConsoleColor` color])
  - [ ] TypeWrite(`String` text, `Int32` interval, [`Action` alsoDo])
- Meta
  - [ ] `String` Serialize(`Array`, [`String` seperator, `Boolean` showIndexes])