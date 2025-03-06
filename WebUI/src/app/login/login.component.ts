import { Component } from '@angular/core';
import { ApiException, APIService, LoginDto } from '../api.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { LoginResponse } from '../app.model';
import { AuthService } from '../auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  isLoading = false;
  formResetToggle?:any;
  isSubmitted = false;
  isModal = false;
  LoginForm!: FormGroup;
  message: any;
  returnUrl: any;

  /**
   *
   */
  constructor(private api:APIService, private fb:FormBuilder, private route:ActivatedRoute,private router:Router,private authService:AuthService) {
    this.LoginForm = this.fb.group({
      email: ["", [Validators.required,Validators.email]],
      password: ["", Validators.required],
      rememberme: [""]
  })
  this.LoginForm.valueChanges.subscribe(x=>{
    this.message='';
  })
  }
  ngOnInit() {
   
}
get f() {
    return this.LoginForm?.controls
}
login() {
  console.log("login clicked");
  this.message='';
  this.LoginForm?.updateValueAndValidity();
    this.isLoading = true;
    this.isSubmitted = true;
    this.api.login({email:this.f['email']?.value,password:this.f['password'].value}).subscribe({
      next:(value:LoginDto)=> {
        console.log(value)
       console.log(this.returnUrl);       
              this.authService.LoggedInUser(value);
               this.returnUrl = this.route.snapshot.queryParams['returnUrl'] || "/"
       this.router.navigate([this.returnUrl]) 
      },
      error:(e?:any)=> {
        console.log(e)
        this.message = e?.message;
        
        this.isLoading = false;
        if(e?.status===402){
          //this.authService.redirectToLockscreen(this.f['username'].value, this.f['password'].value,e)
        }
      }
    
    })
}
ResetPassword() {
    //this.authService.resetPassword("AECU001").subscribe(e=>{}
    //)
}
}
