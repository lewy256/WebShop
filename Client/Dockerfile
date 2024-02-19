FROM node:18.14.0 as build
WORKDIR /usr/local/app
COPY Client/ /usr/local/app/

RUN npm install
RUN npm run build

FROM nginx:latest
COPY --from=build /usr/local/app/dist/client /usr/share/nginx/html

EXPOSE 80
