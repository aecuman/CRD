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
  totalUsers: number = 0;
  users: UserViewModel[]=[];

  isModalOpen: boolean = false; // Controls modal visibility

  registerForm: FormGroup;
  message: string = '';
  errorMessage: string = '';

  isLoading = false;
  isResetting: boolean= false;

  availableRoles = [
    { value: 'admin',                       label: 'Admin',                         desc: 'All actions + user management' },
    { value: 'superadmin',                  label: 'Super Admin',                   desc: 'Full unrestricted access' },
    { value: 'User',                        label: 'User',                          desc: 'View only' },
    { value: 'Registry',                    label: 'Registry',                      desc: 'Document upload & data entry' },
    { value: 'DataInputClerk',              label: 'Data Input Clerk',              desc: 'Rate entry (crops/structures)' },
    { value: 'ChairModerationCommittee',    label: 'Chair – Moderation Committee',  desc: 'Review, approve, schedule, final approval' },
    { value: 'ModerationCommitteeSecretary',label: 'Secretary – Moderation Committee', desc: 'Moderate, generate reports, draft comms' },
    { value: 'RegionalOfficer',             label: 'Regional Officer',              desc: 'Communicate outcomes to/from districts' },
    { value: 'Manager',                     label: 'Manager (legacy)',              desc: 'Alias for Regional Officer' },
  ];

  toggleRole(value: string, event: Event): void {
    const checked = (event.target as HTMLInputElement).checked;
    const current: string[] = this.registerForm.get('roles')?.value ?? [];
    const updated = checked ? [...current, value] : current.filter((r: string) => r !== value);
    this.registerForm.patchValue({ roles: updated });
  }
  constructor(private api: APIService,private fb: FormBuilder) {
    this.registerForm = this.fb.group({
      id:[''],
      firstname: ['', [Validators.required, Validators.minLength(3)]],
      lastname: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      title: [''],
      designation: [''],
      dutyStation: [''],
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
  nextPage() {
    this.pageNumber++;
    this.getUsers();
  }
  
  previousPage() {
    if (this.pageNumber > 1) {
      this.pageNumber--;
      this.getUsers();
    }
  }
  
  goToPage(page: number) {
    if (page >= 1) {
      this.pageNumber = page;
      this.getUsers();
    }
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
      title: user.title,
      designation: user.designation,
      dutyStation: user.dutyStation,
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
      title: this.registerForm.value.title,
      dutyStation: this.registerForm.value.dutyStation,
      designation: this.registerForm.value.designation,
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
