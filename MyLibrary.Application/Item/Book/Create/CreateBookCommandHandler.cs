using MyLibrary.Application.Abstraction.Database;
using MyLibrary.Application.FUTURE_API.Item.Book.Create;
using MyLibrary.Application.Item.Abstraction.Create;
using MyLibrary.Application.Item.Repository;

namespace MyLibrary.Application.Item.Book.Create;

sealed internal class CreateBookCommandHandler(IItemRepository<Domain.Item.Book.Book> itemRepository, IUnitOfWork unitOfWork)
    : CreateItemCommandHandler<Domain.Item.Book.Book, CreateBookCommand, CreateBookResponse>(itemRepository, unitOfWork)
{
    protected override Domain.Item.Book.Book CreateItem(CreateBookCommand request, Guid owner) =>
        Domain.Item.Book.Book.Create(request.Name, request.Author, request.Year, request.Isbn, request.Description, owner);

    protected override CreateBookResponse CreateResponse(Domain.Item.Book.Book item) =>
        new(item.Id, item.Name, item.Description, item.Owner);
}