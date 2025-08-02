# Learn about building .NET container images:
# https://github.com/dotnet/dotnet-docker/blob/main/samples/README.md
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /source

RUN mkdir ShoppingListApi
RUN mkdir Shared

# Copy project file and restore as distinct layers
COPY --link ShoppingListApi/*.csproj ./ShoppingListApi
COPY --link ShoppingListApi/. ./ShoppingListApi


# Copy project file and restore as distinct layers
COPY --link Shared/*.csproj ./Shared
COPY --link Shared/. ./Shared

WORKDIR /source/ShoppingListApi

# Copy source code and publish app
RUN dotnet restore
RUN dotnet publish -o /app


# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS shopping-list-api
EXPOSE 8080
WORKDIR /app
COPY --link --from=build /app .
USER $APP_UID
ENTRYPOINT ["./ShoppingListApi"]
