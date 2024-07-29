# Set the base image as the .NET 6.0 SDK (this includes the runtime)
FROM mcr.microsoft.com/dotnet/sdk:8.0 as build-env

# Copy everything and publish the release (publish implicitly restores and builds)
WORKDIR /app
COPY . ./
RUN dotnet publish ./MyBlueprint.ResxConverter.csproj -c Release -o out --no-self-contained

# Label the container
LABEL maintainer="admin"
LABEL repository="https://github.com/myblueprint-spaces/resx-converter"
LABEL homepage="https://github.com/myblueprint-spaces/resx-converter"

# Label as GitHub action
LABEL com.github.actions.name="ResxConverter"
LABEL com.github.actions.description="Convert csv to resx."
LABEL com.github.actions.icon="activity"
LABEL com.github.actions.color="orange"

# Relayer the .NET SDK, anew with the build output
FROM mcr.microsoft.com/dotnet/sdk:8.0
COPY --from=build-env /out .
ENTRYPOINT [ "dotnet", "/MyBlueprint.ResxConverter.dll" ]
