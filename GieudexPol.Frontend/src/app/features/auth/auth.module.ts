import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
// Importowanie komponentów jako standalone (jeśli są to faktycznie standalone)
import { LoginComponent } from './login/login.component';
import { RegisterComponent } from './register/register.component';

@NgModule({
  imports: [
    CommonModule,
    LoginComponent, // Imporujemy do modułu używającego AuthModule (np. AppRoutingModule)
    RegisterComponent, // I tak samo tutaj
    // ... inne moduły
  ],
  declarations: [], // Usunięto deklaracje komponentów standalone z NgModule
  providers: [],
  bootstrap: [/*...*/]
})
export class AuthModule { }
