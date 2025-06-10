#!/bin/bash

# Banking API Startup Script with HTTPS
echo "🏦 Starting Banking API with HTTPS..."
echo "📍 Location: Banking.API"

# Navigate to the Banking.API directory
cd Banking.API

# Run the application with HTTPS profile
echo "🚀 Running: dotnet run --launch-profile https"
echo "🌐 HTTPS URL: https://localhost:7086"
echo "📄 Swagger UI: https://localhost:7086/swagger"
echo "⚡ HTTP URL: http://localhost:5281 (redirects to HTTPS)"
echo ""
echo "Press Ctrl+C to stop the server"
echo "----------------------------------------"

dotnet run --launch-profile https 