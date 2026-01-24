const items = document.querySelectorAll('.task-item');
items.forEach(item => {
    item.draggable = true;
    item.addEventListener('dragstart', e => e.dataTransfer.setData('text/plain', item.id));
    item.addEventListener('drop', e => {
        const draggedId = e.dataTransfer.getData('text/plain');
        DotNet.invokeMethodAsync('YourAssemblyName', 'OnDrop', parseInt(draggedId), parseInt(item.id));
    });
});


