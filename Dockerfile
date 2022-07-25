FROM nginx as production

ENV NODE_ENV=production

COPY --from=react-app-prod /app/launcher/build /usr/share/nginx/html

COPY nginx.conf /etc/nginx/nginx.conf

COPY nginx.conf /etc/nginx/nginx.conf:ro

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]