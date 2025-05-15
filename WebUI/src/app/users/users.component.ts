import { Component } from '@angular/core';
import { APIService, RegisterUserCommand, UpdateUserCommand, UserViewModel } from '../api.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-users',
  standalone: false,
  templateUrl: './users.component.html',
  styleUrl: './users.component.css'
})
export class UsersComponent {
  /**
   *
   */
  pageNumber: number = 1;
  pageSize: number = 10;
  users: UserViewModel[]=[];

  isModalOpen: boolean = false; // Controls modal visibility

  registerForm: FormGroup;
  message: string = '';
  errorMessage: string = '';

  isLoading = false;
  isResetting: boolean= false;
  constructor(private api: APIService,private fb: FormBuilder) {
    this.registerForm = this.fb.group({
      id:[''],
      firstname: ['', [Validators.required, Validators.minLength(3)]],
      lastname: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      role:[],
      roles: [[], Validators.required], // Ensure at least one role is selected
    });
    this.getUsers();
   
    
  }
  getUsers() {
    this.api.getUsers(this.pageNumber,this.pageSize).subscribe((data: UserViewModel[]) => {
      console.log(data);
      this.users = data;
    });
  }
  editUser(user: UserViewModel) {
    this.isModalOpen = true;
    this.registerForm.reset();
    this.message = '';
    this.errorMessage = '';
    this.registerForm.patchValue({
      id: user.id,
      firstname: user.firstname,
      lastname: user.lastname,
      email: user.email,
      role: user.role,
      roles: user.role?.split(", "), // Assuming roles is an array of strings
    });
    //this.registerForm.get('roles')?.setValue(user.roles); // Set the selected roles
  }
  ResetPassword(user: UserViewModel) {
    if(confirm("Are you sure you want to reset the password for "+user.fullName))
      this.isResetting = true
    this.api.forgotPassword({email:user.email}).subscribe((data: any) => {
  alert(user.fullName+" password reset link sent to "+user.email);
      this.isResetting = false;
      //console.log(data);
    });
  }
  openModal(): void {
    this.isModalOpen = true;
    this.registerForm.reset();
    this.message = '';
    this.errorMessage = '';
  }

  closeModal(): void {
    this.isModalOpen = false;
  }
  submit(): void {
    this.isLoading = true;
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched(); // Show errors when submitting an invalid form
      return;
    }
    if(this.registerForm.value.id) {
var payload:UpdateUserCommand={
  id: this.registerForm.value.id,
      firstname: this.registerForm.value.firstname,
      lastname: this.registerForm.value.lastname,
      email: this.registerForm.value.email,
      role: this.registerForm.value.role,
      roles: this.registerForm.value.roles
    };


      this.api.editUser(payload).subscribe({
        next:(res: any) => {
          this.isLoading = false;
          this.message = res.message;
          this.errorMessage = '';
          this.getUsers(); // Refresh the user list after updating
          setTimeout(() => {
            this.closeModal();
          }, 2000);
        },
        error:(error: any) => {
          this.isLoading = false;
          this.errorMessage = error.error.message || 'Failed to update user';
          this.message = '';
        }
      });
    }else{
 var body:RegisterUserCommand={
      firstname: this.registerForm.value.firstname,
      lastname: this.registerForm.value.lastname,
      email: this.registerForm.value.email,
      roles: this.registerForm.value.roles,
    };
    console.log(body);
    if (this.registerForm.invalid) {
      this.isLoading = false;
      return;
 }
    this.api.newUser(body).subscribe({ 
      next:(res: any) => {
        this.isLoading = false;
        this.message = res.message;
this.errorMessage = '';
this.getUsers(); // Refresh the user list after creating a new user
//setTimeout(() => {
  this.closeModal();
 
//}, 2000);
       },
      error:(error: any) => {
        this.isLoading = false;
        this.errorMessage = error.error.message || 'Failed to create user';
        this.message = '';
      }
    }
    );
  }
}
}
