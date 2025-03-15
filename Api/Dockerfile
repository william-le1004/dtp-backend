# Learn about building .NET container images:
# https://github.com/dotnet/dotnet-docker/blob/main/samples/README.md
FROM --platform=$BUILDPLATFORM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG TARGETARCH
WORKDIR /source

# Copy the .csproj files first to restore dependencies
COPY --link Api/Api.csproj Api/
COPY --link Domain/Domain.csproj Domain/
COPY --link Infrastructure/Infrastructure.csproj Infrastructure/
COPY --link Application/Application.csproj Application/

# Restore dependencies as distinct layers
RUN dotnet restore Api/Api.csproj -a $TARGETARCH

# Now copy the rest of the source code
COPY --link . .

# Publish the app
RUN dotnet publish Api/Api.csproj -a $TARGETARCH --no-restore -o /app

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0
EXPOSE 8080
WORKDIR /app

# Copy the build output from the previous stage
COPY --link --from=build /app . 

# Optionally, if you need to run the app as a non-root user
USER 1000  # You can replace 1000 with your specific user ID if needed

# Set the entry point for the application
ENTRYPOINT ["dotnet", "Api.dll"]
