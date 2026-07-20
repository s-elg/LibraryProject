using FluentValidation;
using LibraryProject.Application.DTOs;

namespace LibraryProject.Application.Validators
{
    public class CreateBookRequestValidator : AbstractValidator<CreateBookRequestDto>
    {
        public CreateBookRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Kitap başlığı boş olamaz.")
                .MaximumLength(200).WithMessage("Kitap başlığı en fazla 200 karakter olabilir.");

            RuleFor(x => x.Author)
                .NotEmpty().WithMessage("Yazar adı boş olamaz.")
                .MaximumLength(150).WithMessage("Yazar adı en fazla 150 karakter olabilir.");

            RuleFor(x => x.ISBN)
                .NotEmpty().WithMessage("ISBN boş olamaz.")
                .Matches(@"^(?:\d{10}|\d{13})$").WithMessage("ISBN 10 veya 13 haneli rakamlardan oluşmalı.");

            RuleFor(x => x.TotalCopies)
                .GreaterThan(0).WithMessage("Toplam kopya sayısı 0'dan büyük olmalı.");

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Kategori seçilmelidir.");
        }
    }

    public class UpdateBookRequestValidator : AbstractValidator<UpdateBookRequestDto>
    {
        public UpdateBookRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Kitap başlığı boş olamaz.")
                .MaximumLength(200).WithMessage("Kitap başlığı en fazla 200 karakter olabilir.");

            RuleFor(x => x.Author)
                .NotEmpty().WithMessage("Yazar adı boş olamaz.")
                .MaximumLength(150).WithMessage("Yazar adı en fazla 150 karakter olabilir.");

            RuleFor(x => x.ISBN)
                .NotEmpty().WithMessage("ISBN boş olamaz.")
                .Matches(@"^(?:\d{10}|\d{13})$").WithMessage("ISBN 10 veya 13 haneli rakamlardan oluşmalı.");

            RuleFor(x => x.TotalCopies)
                .GreaterThan(0).WithMessage("Toplam kopya sayısı 0'dan büyük olmalı.");

            RuleFor(x => x.CategoryId)
                .NotEmpty().WithMessage("Kategori seçilmelidir.");
        }
    }
}