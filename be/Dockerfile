# Usa l'immagine base di .NET 8.0 SDK per la build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia i file di progetto e ripristina le dipendenze
COPY ["Banking.API/Banking.API.csproj", "Banking.API/"]
COPY ["Banking.Models/Banking.Models.csproj", "Banking.Models/"]
COPY ["Banking.Logic/Banking.Logic.csproj", "Banking.Logic/"]
COPY ["Banking.Infrastructure/Banking.Infrastructure.csproj", "Banking.Infrastructure/"]

RUN dotnet restore "Banking.API/Banking.API.csproj"

# Copia tutto il codice sorgente
COPY . .

# Build dell'applicazione
WORKDIR "/src/Banking.API"
RUN dotnet build "Banking.API.csproj" -c Release -o /app/build

# Pubblica l'applicazione
FROM build AS publish
RUN dotnet publish "Banking.API.csproj" -c Release -o /app/publish

# Usa l'immagine runtime per l'esecuzione
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /persistence

# Installa openssl per generare certificati
RUN apt-get update && apt-get install -y openssl curl && rm -rf /var/lib/apt/lists/*

# Crea directory per i certificati
RUN mkdir -p /persistence/certs

# Genera certificato self-signed
RUN openssl req -x509 -newkey rsa:4096 -keyout /persistence/certs/banking-api.key -out /persistence/certs/banking-api.crt -days 365 -nodes -subj "/C=IT/ST=Italy/L=City/O=Banking/OU=API/CN=banking-api"

# Converti in formato PFX per .NET
RUN openssl pkcs12 -export -out /persistence/certs/banking-api.pfx -inkey /persistence/certs/banking-api.key -in /persistence/certs/banking-api.crt -password pass:password

# Copia i file pubblicati
COPY --from=publish /app/publish .

# Esponi le porte
EXPOSE 80
EXPOSE 443

# Configura le variabili d'ambiente per HTTPS
ENV ASPNETCORE_URLS="https://+:443;http://+:80"
ENV ASPNETCORE_Kestrel__Certificates__Default__Path="/persistence/certs/banking-api.pfx"
ENV ASPNETCORE_Kestrel__Certificates__Default__Password="password"

ENTRYPOINT ["dotnet", "Banking.API.dll"] 