using FluentValidation;
using LibraryProject.Application.DTOs;

namespace LibraryProject.Application.Validators
{
    public class CreateCategoryValidator : AbstractValidator<CreateCategoryDto>
    {
        public CreateCategoryValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Kategori adı boş olamaz.")
                .MinimumLength(2).WithMessage("Kategori adı en az 2 karakter olmalı.")
                .MaximumLength(100).WithMessage("Kategori adı en fazla 100 karakter olabilir.");
        }
    }

    public class UpdateCategoryValidator : AbstractValidator<UpdateCategoryDto>
    {
        public UpdateCategoryValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Kategori adı boş olamaz.")
                .MinimumLength(2).WithMessage("Kategori adı en az 2 karakter olmalı.")
                .MaximumLength(100).WithMessage("Kategori adı en fazla 100 karakter olabilir.");
        }
    }
}