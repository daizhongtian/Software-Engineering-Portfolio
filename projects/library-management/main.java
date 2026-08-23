package library;

import java.util.HashMap;
import java.util.List;
import java.util.Scanner;




public class main {

    public static void main(String[] args) {

        

          System.out.println("\n===== Library Management Menu =====");
            System.out.println("1. Add Book");
            System.out.println("2. List Books");
            System.out.println("3. Return Book");
            System.out.println("4. borrow books");
            System.out.println("5. extend retrun deadline");


            System.out.println("6. Exit");


               Scanner scanner = new Scanner(System.in);
                 library library = new library();


            while(true)
            {
               System.out.print("Enter your choice: ");
               int choice =scanner.nextInt();
               scanner.nextLine();

                

               switch(choice)
               {
                case 1:
                System.out.println("please enter name");
                String nametmp = scanner.nextLine().trim();
                System.out.println("please enter author");
                String authortmp = scanner.nextLine().trim();
                if(nametmp.isEmpty()||authortmp.isEmpty())
                {
                    System.out.println("name and author cannot be empty");
                    break;
                }
                
                Book book = new Book(nametmp,authortmp);
                try{
                  library.addBook(book);
                  System.out.println("book added"+book);

                  
                }
                catch(IllegalArgumentException e) {
                    System.out.println("Failed to add book: " + e.getMessage());
                }
                break;
                

                  case 2:
                  
                  library.displayallbooks(library);

                  break;

                  case 3:

                  while(true)
                  {

                  
                    library.displayallbooks(library);
                    System.out.println("enter the id you want borrow");
                    if(!scanner.hasNextInt())
                    {
                      System.out.println("invalid id");
                      scanner.nextLine();
                      continue;
                    }
                    int choiceToBorrow = scanner.nextInt();
                    scanner.nextLine();
                    System.out.println("enter borrower's name");
                    String borrowername =scanner.nextLine().trim();
                    if(borrowername.isEmpty())
                    {
                      System.out.println("name cannot be empty");
                      continue;
                    }
                    System.out.println("enter the number of days to borrow");
                    int borrowdays = scanner.nextInt();
                    scanner.nextLine();
                    if(borrowdays<=0)
                    {
                      System.out.println("Invalid number. Please enter a positive integer.");
                        continue;
                    }


                    library.borrow(choiceToBorrow,borrowername,borrowdays);
                    break;

                  }
                  break;
                 
                    



                  case 4:
                  
                    library.displayallbooks(library);
                    while(true)
                    {
                    System.out.println("enter book id you want return");
                    if(!scanner.hasNextInt())
                    {
                      System.out.println("invalid id");
                      scanner.nextLine();
                      continue;
                    }
                    int choicereturn = scanner.nextInt();
                    scanner.nextLine();
                    boolean ok=library.giveBack(choicereturn);
                    if(ok)
                    {
                                  System.out.println("Return successful.");
                            break; 

                    }
                    else{
                 System.out.println("Return failed: book not found or not currently borrowed.");

                    }
                    



                  }
                  break;

                  






               }


               
            }


        
    }
    
}
