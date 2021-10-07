![](icon.png "MTLibrary C#")
# C# MTLibrary *v0.5.27.21*

- [x] DictionaryFile
  - Acts as a controller for a File-synchronized
    `Dictionary<String, String>` object.
  - Useful for easily persisting `String` pairs in a locally-stored binary file format.
  - When assigning a new value, the binary file will still not contain the it.
    
    Call `Save` in order to synchronize new keys/value inside the binary.
    However, if you were to try to `Get` an unsynchronized key, this controller object
    will search it's internal `Dictionary` for the key, and return it (even if the
    binary does not yet contain it). Furthermore, if the key does not exist,
    the `Get` call will return a `String.Empty`.
    
    To clarify, if the binary contains a key that is not synchronized, calling
    `Get` will not `Load` the binary and return the already-saved key;
    instead, it will return `String.Empty`, because the binary
    has not yet been instructed to synchronize (with `Load`).
  - `constructor` this(path)
  - `String` this[`int` index]
  - `String` this[`String` key]
  - `String` Get(`String` key)
  - `void` Remove(`String` key)
  - `Boolean` IsKey(`String` key)
  - `Boolean` Contains(`String` value)
  - `void` Set(`String` key, [`String` value = `String.Empty`]) 
  - `void` Load()
  - `void` Clear()
  - `void` Save()
- [ ] `struct` Networking
  - [ ] Make this `static`.
  - [ ] Redo this section with `Async` awareness. 
  - [ ] `class` Server
    - [ ] `constructor` (`delegate void` (`Socket` newConnection))
    - [ ] `void` Send(`Socket` s, `Transmission` t)
    - [ ] `void` Send(`Socket` s, `Byte[]` data)
  - [ ] `class` Client
    - [ ] `constructor` Client(`System.Net.IPAddress` ip, `int` port)
    - [ ] `void` Send(`Byte[]` data)
  - [ ] `class` Transmission
    - [x] `constructor` (`Socket` listenTo)
    - [x] `constructor` (`Byte[]` data)
    - [ ] `void` Set(`String` key, `String` value)
    - [ ] `String` Get(`String` key)
    - [x] `Boolean` Send(`Socket` to)
- [ ] Display
  - [x] Write(`String` text, [`ConsoleColor` color])
  - [ ] TypeWrite(`String` text, `Int32` interval, [`Action` alsoDo])
- [ ] Meta
  - `String` Author, Email, Codename
  - `ConsoleColor` ColorCode
  - [ ] `String` Serialize(`Array` target, [`String` seperator, `Boolean` showIndexes])