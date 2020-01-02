FROM nginx:latest

USER root

# Remove the default Nginx configuration file
RUN rm -v /etc/nginx/nginx.conf

# Copy a configuration file from the current directory
ADD nginx.conf /etc/nginx/

ADD web /usr/share/nginx/html/
ADD web /var/www/html/

# Expose ports
EXPOSE 90

CMD ["nginx", "-g", "daemon off;"]