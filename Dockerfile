# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia o conteúdo do projeto
COPY . .

# Restaura e publica
RUN dotnet restore API_Miniapp_Gestao/API_Miniapp_Gestao.csproj
RUN dotnet publish API_Miniapp_Gestao/API_Miniapp_Gestao.csproj -c Release -o /app/publish

# Etapa de runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Define variáveis de ambiente
ENV ASPNETCORE_HTTP_PORTS=8080
ENV TZ="America/Sao_Paulo"

# Expõe portas
EXPOSE 8080
EXPOSE 443

# Copia os arquivos publicados
COPY --from=build /app/publish .

# Define o ponto de entrada
ENTRYPOINT ["dotnet", "API_Miniapp_Gestao.dll"]
