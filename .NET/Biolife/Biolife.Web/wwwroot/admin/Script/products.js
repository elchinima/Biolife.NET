document.querySelectorAll('.btn-edit-product').forEach(btn => {
    btn.addEventListener('click', () => {
        const data = btn.dataset;

        document.getElementById('editProductId').value = data.id || '';
        document.getElementById('editProductName').value = data.name || '';
        document.getElementById('editProductPrice').value = data.price || '';
        document.getElementById('editProductCostPrice').value = data.costprice || '';
        document.getElementById('editProductDiscountPercent').value = data.discountpercent || '';
        document.getElementById('editProductTagId').value = data.tagid || '';
        document.getElementById('editProductGenreId').value = data.genreid || '';
        document.getElementById('editProductDescription').value = data.description || '';
        document.getElementById('editIsFeatured').checked = data.isfeatured === 'true';
        document.getElementById('editIsNew').checked = data.isnew === 'true';

        const editModal = document.getElementById('editModal');
        if (!editModal) return;

        editModal.querySelectorAll('input[type="file"]').forEach(input => {
            input.value = '';
            input.dispatchEvent(new Event('change', { bubbles: true }));
        });

        editModal.classList.add('open');
    });
});

