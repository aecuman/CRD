# Use an official Node.js image for building the app
FROM node:18 AS build

WORKDIR /app
COPY package.json package-lock.json ./
RUN npm install

COPY . .
RUN npm run build --prod

# Use an Nginx image to serve the built Angular app
FROM nginx:latest
COPY --from=build /app/dist/web-ui /usr/share/nginx/html

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
