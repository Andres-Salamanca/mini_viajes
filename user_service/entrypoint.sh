#!/bin/bash
#!/bin/bash

DB_FILE="mini_user.db"

if [ ! -f "$DB_FILE" ]; then
    echo "Database not found. Running migrations..."
    dotnet ef database update
else
    echo "Database already exists. Skipping migrations."
fi

# Inicia la app
dotnet user_service.dll


