# EasyJWT

**EasyJWT** is an enterprise‑grade, modular JSON Web Token library for .NET 9+  

> A drop‑in, standards‑compliant JWT solution with pluggable cryptography, multi‑tenant support, built‑in key rotation and JWKS, extensible validation pipelines, and first‑class ASP.NET Core, gRPC, and SignalR integration.

---

## ✨ Features

- Issue and validate all common JWT types (Access, Refresh, ID, Back‑Channel Logout) and custom token policies  
- Fluent `JwtTokenBuilder` API for effortless token creation  
- Pluggable `IKeyProvider` implementations (In‑Memory, File System, Azure Key Vault, AWS KMS, GCP KMS, Database)  
- Automatic key rotation with public JWKS endpoint  
- Extensible validation pipeline via `IJwtValidator` (for revocation lists, business rules, compliance checks)  
- First‑class multi‑tenant configuration and tenant‑aware key selection  
- ASP.NET Core integration: middleware, `[AuthorizeWithJwt]` attribute, health checks  
- gRPC interceptors and SignalR support for JWT authentication  
- Structured logging events (TokenIssued, ValidationSucceeded, ValidationFailed, TokenRevoked) plus OpenTelemetry & Serilog ready  
- CLI tool to inspect, decode and issue JWTs for debugging and testing  
- Lightweight, zero runtime reflection, MIT licensed  

---

## 📦 Installation

Available on NuGet:

```bash
dotnet add package EasyJWT
