package library;
import java.util.List;
import java.util.ArrayList;
import java.util.Collections;
import java.util.HashMap;
import java.util.Map;

public class library {
    private List<Book> books = new ArrayList<>();
    private Map<Integer, Book> booksById = new HashMap<>();


    public void addBook(Book b )
    {
        
        if(b==null)
        {
            throw new IllegalArgumentException("Book cannot be null");

        }

        if(booksById.containsKey(b.getId()))
        {
           throw new IllegalArgumentException("id is duplicated"+b.getId());
        }

        books.add(b);
        booksById.put(b.getId(),b);
    }

    public Book findByid(int id)
    {
        return booksById.get(id);  //hash map method O(1)
    }


    public List<Book> searchByTitle(String keyword)
    {
        if(keyword==null)
        {
            return Collections.emptyList();
        }
        String k = keyword.trim().toLowerCase();
        if(k.isEmpty())
        {
            return Collections.emptyList();
        }
        List<Book>out = new ArrayList<>();
        for(Book b :books)
        {
            String name =b.GetBookName();
            if(name==null)
            {
                continue;
            }
            if(b.GetBookName().toLowerCase().contains(k))
            {
                out.add(b);

            }
            
        }
        return out;


    }

    public boolean borrow(int bookId,String member,int days)
    {
        Book b =booksById.get(bookId);
        if(b==null||b.isBorrowed())
        {
            return false;
        }
        b.borrow(member, days);
        System.out.println("borrowed successfully");
        return true;

       
    }

    public boolean giveBack(int bookId)
    {
        Book b = booksById.get(bookId);
        if(b==null||!b.isBorrowed())
        {
            return false;
        }
        b.returnbook();
        return true;
    }

    public List<Book> listAll()
    {
        return books;
    }


    public static void displayallbooks(library lib)
    {
    System.out.println("All books:");
    List<Book> allbooks = lib.listAll();

    if (allbooks.isEmpty()) {
        System.out.println("No books in library.");
    } else {
        for (int i = 0; i < allbooks.size(); i++) {
            System.out.println((i + 1) + ". " + allbooks.get(i));
        }
    }
}



}
