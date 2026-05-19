FROM node:22-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
# BACKEND_URL defaults to /api for local Docker Compose (nginx proxy handles it)
# On Render, pass the actual API URL so Angular calls it directly (no nginx proxy hop)
ARG BACKEND_URL=/api
RUN sed -i "s|apiUrl: '/api'|apiUrl: '${BACKEND_URL}'|g" src/environments/environment.production.ts
RUN npm run build

FROM nginx:alpine AS final
RUN apk add --no-cache bash
COPY --from=build /app/dist/sponsorship-app/browser /usr/share/nginx/html
COPY nginx.conf /etc/nginx/templates/default.conf.template
EXPOSE 80
CMD ["/bin/sh", "-c", "envsubst '${API_URL}' < /etc/nginx/templates/default.conf.template > /etc/nginx/conf.d/default.conf && nginx -g 'daemon off;'"]
