FROM mcr.microsoft.com/dotnet/core/sdk:3.0 AS build
WORKDIR /app
COPY ["src/JHipsterNet/JHipsterNet.csproj", "src/JHipsterNet/"]
COPY ["src/Dev1/Dev1.csproj", "src/Dev1/"]
RUN dotnet restore "src/Dev1/Dev1.csproj"
COPY . ./
WORKDIR /app/src/Dev1
RUN apt-get update -yq && apt-get install -yq curl
RUN curl -sL https://deb.nodesource.com/setup_12.x | bash - && \
    apt-get update && \
    apt-get install -yq nodejs && \
    rm -rf /var/lib/apt/lists/*
RUN npm install
RUN rm -rf wwwroot && \
    dotnet publish "Dev1.csproj" -c Release -o /app/out

FROM mcr.microsoft.com/dotnet/core/aspnet:3.0 AS runtime
WORKDIR /app
EXPOSE 80
COPY --from=build /app/out .
ENTRYPOINT ["dotnet", "dev-1.dll"]
