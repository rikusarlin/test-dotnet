# Very basics of .Net and C#
This repo is nothing fancy - basic Rest service written in C# and .Net (version 10), using modern conventions.

This is a classic "ToDo" backend service, with some unit and integration tests in xUnit.

## Testing
```
dotnet test
```

## Running
First in one window
```
dotnet run --project MyApi/MyApi.csproj
```

Then in other window do something like
```
curl -X POST http://localhost:5005/todos \
  -H "Content-Type: application/json" \
  -d '{"title": "Learn C# v2"}'

curl -X GET http://localhost:5005/todos 
```


## Building and running Native Apple Silicon executable! Yes that's possible!
First build the thing with
```
dotnet publish -r osx-arm64 -c Release --self-contained true /p:PublishSingleFile=true -o ./publish
```

...and then you can run it as local binary
```
./MyApi/publish/MyApi 
```

...and then you can test it. Note that port differs from "development build" by default.
```
curl -X POST http://localhost:5000/todos \
  -H "Content-Type: application/json" \
  -d '{"title": "Learn C# v2"}'

curl -X GET http://localhost:5000/todos 
```


