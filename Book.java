package library;

import java.time.Instant;
import java.time.temporal.ChronoUnit;
import java.util.concurrent.atomic.AtomicInteger;

public class Book {

  private static final AtomicInteger idCounter = new AtomicInteger(1);


    private final int id;
    private final String bookname;   // 原来的 BookName
    private final String author;

    // 借阅状态（初始化时默认为未借出）
    private boolean borrowed = false;
    private String borrower = null;
    private Instant borrowedTime = null;
    private Instant dueTime = null; // 可选：记录到期时间

  

    public String GetAuthor()
    {
        return author;
    }

    public String GetBookName()
    {
        return bookname;
    }

        public Instant getBorrowedTime() 
        {
        return borrowedTime;
    }

    public Book(String BookName, String Author)
    {
        this.id = idCounter.getAndIncrement();
        this.bookname = BookName;
        this.author = Author;
     

    }

 

    public void borrow(String member,int days)

    {
      if(borrowed)
      {
        throw new IllegalStateException("book is already borrowed");
      }
      this.borrowed = true;
      this.borrower = member;
      this.borrowedTime = Instant.now();
      this.dueTime = borrowedTime.plus(days, ChronoUnit.DAYS);
    }

    public boolean isBorrowed()
    {
      return borrowed;
    }

    public void returnbook()
    {
      this.borrowed=false;
      this.borrower =null;
      this.dueTime=null;
      this.borrowedTime=null;
    }

    @Override
    public String toString()
    {
      String base = String.format ("id=%d,title=%s,author=%s",id,bookname,author);

      if(!borrowed)
      {
        return base+"(avaliable)";
      }
      String who = (borrower==null||borrower.isEmpty())? "unknown":borrower;
      String when =(borrowedTime==null)?"N/A" :borrowedTime.toString();
      return base +String.format("(BORROWED by %s at %s)",who,when);


    }

    public int getId()
    {
      return id;

    }













    
}
