# Stage 1: Build dell'applicazione Angular
FROM node:18-alpine AS build

WORKDIR /app

# Copia package.json e package-lock.json
COPY package*.json ./

# Installa le dipendenze
RUN npm ci

# Copia tutto il codice sorgente
COPY . .

# Build dell'applicazione Angular per production
RUN npm run build:prod

# Stage 2: Serve l'applicazione con nginx
FROM nginx:alpine

# Installa wget per healthcheck
RUN apk add --no-cache wget

# Copia la configurazione personalizzata di nginx
COPY nginx.conf /etc/nginx/nginx.conf

# Copia i file buildati dall'applicazione Angular
COPY --from=build /app/dist/banking-poc /usr/share/nginx/html

# Copia script per configurazione dinamica
COPY docker-entrypoint.sh /docker-entrypoint.sh
RUN chmod +x /docker-entrypoint.sh

# Esponi la porta 80
EXPOSE 80

# Usa lo script di entrypoint per configurazione dinamica
ENTRYPOINT ["/docker-entrypoint.sh"]
CMD ["nginx", "-g", "daemon off;"] 