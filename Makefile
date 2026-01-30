.PHONY: help build up down logs clean

help:
	@echo "Comandos disponibles:"
	@echo "  make build     - Construye las imágenes Docker"
	@echo "  make up        - Inicia los contenedores"
	@echo "  make down      - Detiene los contenedores"
	@echo "  make logs      - Muestra los logs"
	@echo "  make clean     - Limpia contenedores e imágenes"

build:
	docker-compose build

up:
	docker-compose up -d

down:
	docker-compose down

logs:
	docker-compose logs -f

clean:
	docker-compose down -v
