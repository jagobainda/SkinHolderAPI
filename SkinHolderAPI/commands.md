# Scaffolding
```PowerShell
Scaffold-DbContext "server=localhost;port=3306;database=SkinHolderDB;user=root;password=root" Pomelo.EntityFrameworkCore.MySql -OutputDir Models -Context SkinHolderDbContext -ContextDir DataService/Contexts -Namespace SkinHolderAPI.Models -ContextNamespace SkinHolderAPI.DataService.Contexts -NoOnConfiguring -Force
```

# Scaffolding log
```PowerShell
Scaffold-DbContext "server=localhost;port=3306;database=SkinHolderLog;user=root;password=root" Pomelo.EntityFrameworkCore.MySql -OutputDir Models/Logs -Context SkinHolderLogDbContext -ContextDir DataService/Contexts -Namespace SkinHolderAPI.Models.Logs -ContextNamespace SkinHolderAPI.DataService.Contexts -NoOnConfiguring -Force
```

# Run MySQL service
```PowerShell
net start MySQL80
```