const $ = (selector) => document.querySelector(selector);
const cache = { dishes: [], customers: [], orders: [], tables: [], cart: { items: [], total: 0 } };
let noticeTimer;
let dishSearchTimer;

// Browser actions use the same-origin JSON API; domain validation remains on the server.
async function api(path, options = {}) {
  const response = await fetch(`/api${path}`, {
    ...options,
    headers: { ...(options.body ? { 'Content-Type': 'application/json' } : {}), ...options.headers }
  });
  if (!response.ok) {
    let message = `Помилка сервера (${response.status})`;
    try { message = (await response.json()).message || message; } catch { /* Some server errors have no JSON body. */ }
    throw new Error(message);
  }
  return response.status === 204 ? null : response.json();
}

function showNotice(message, isError = false) {
  const notice = $('#notice');
  notice.textContent = message;
  notice.classList.toggle('error', isError);
  notice.hidden = false;
  clearTimeout(noticeTimer);
  noticeTimer = setTimeout(() => { notice.hidden = true; }, 4500);
}

function cell(value) {
  const td = document.createElement('td');
  td.textContent = value ?? '';
  return td;
}

function actionButton(label, title, action, danger = false) {
  const button = document.createElement('button');
  button.type = 'button';
  button.className = danger ? 'row-action danger' : 'row-action';
  button.textContent = label;
  button.title = title;
  button.setAttribute('aria-label', title);
  button.addEventListener('click', action);
  return button;
}

async function loadDashboard() {
  const summary = await api('/restaurant');
  $('#restaurant-name').textContent = summary.name;
  $('#restaurant-meta').textContent = `${summary.address} · ${summary.cuisine} кухня`;
  const values = [
    ['Страви в меню', summary.dishes, 'Позицій у каталозі'],
    ['Клієнти', summary.customers, 'Зареєстровано гостей'],
    ['Замовлення', summary.orders, `${summary.pendingOrders} очікують оплати`],
    ['Дохід', `${Number(summary.revenue).toFixed(2)} грн`, `${summary.paidOrders} оплачених замовлень`],
    ['Вільні столики', `${summary.availableTables} / ${summary.tables}`, 'Доступно для бронювання'],
    ['Середня ціна', `${Number(summary.averageDishPrice).toFixed(2)} грн`, summary.mostExpensiveDish ? `Найдорожча страва: ${summary.mostExpensiveDish}` : 'Меню порожнє']
  ];
  $('#stats').replaceChildren(...values.map(([label, value, foot]) => {
    const card = document.createElement('article'); card.className = 'stat-card';
    const title = document.createElement('div'); title.className = 'stat-label'; title.textContent = label;
    const count = document.createElement('div'); count.className = 'stat-value'; count.textContent = value;
    const caption = document.createElement('div'); caption.className = 'stat-foot'; caption.textContent = foot;
    card.append(title, count, caption); return card;
  }));
  cache.orders = await api('/orders');
  renderOrders($('#recent-orders'), cache.orders.slice(-5).reverse(), false);
}

async function loadDishes() {
  const params = new URLSearchParams();
  if ($('#dish-search').value.trim()) params.set('search', $('#dish-search').value.trim());
  if ($('#dish-category').value) params.set('category', $('#dish-category').value);
  params.set('sort', $('#dish-sort').value);
  cache.dishes = await api(`/dishes?${params}`);
  const rows = cache.dishes.map(dish => {
    const row = document.createElement('tr');
    const name = cell('');
    const detail = document.createElement('button'); detail.type = 'button'; detail.className = 'link-button';
    detail.textContent = dish.name; detail.addEventListener('click', () => showDishDetails(dish.id));
    name.append(detail);
    row.append(name, cell(dish.category), cell(`${Number(dish.price).toFixed(2)} грн`), cell(`${dish.weight} г`));
    const actions = document.createElement('td'); actions.className = 'row-actions';
    actions.append(
      actionButton('＋ Кошик', `Додати ${dish.name} до кошика`, () => addToCart(dish.id)),
      actionButton('Змінити', 'Редагувати страву', () => editDish(dish)),
      actionButton('Видалити', 'Видалити страву', () => deleteDish(dish.id), true)
    );
    row.append(actions); return row;
  });
  if (!rows.length) {
    const row = document.createElement('tr'); const empty = cell('За заданими умовами страв не знайдено.');
    empty.colSpan = 5; row.append(empty); rows.push(row);
  }
  $('#dishes-body').replaceChildren(...rows);

  const allDishes = await api('/dishes?sort=name');
  const categories = [...new Set(allDishes.map(dish => dish.category))].sort((a, b) => a.localeCompare(b, 'uk'));
  const categorySelect = $('#dish-category');
  const selectedCategory = categorySelect.value;
  categorySelect.replaceChildren(new Option('Усі категорії', ''), ...categories.map(category => new Option(category, category)));
  categorySelect.value = selectedCategory;
  fillSelect($('#order-dish'), allDishes, dish => dish.name);
}

async function showDishDetails(id) {
  try {
    const dish = await api(`/dishes/${id}`);
    $('#dish-details-title').textContent = dish.name;
    const values = [
      ['Категорія', dish.category], ['Ціна', `${Number(dish.price).toFixed(2)} грн`],
      ['Вага', `${dish.weight} г`], ['Опис', dish.description],
      ['Акційна ціна за правилами страви', `${Number(dish.promotionalPrice).toFixed(2)} грн`]
    ];
    $('#dish-details-body').replaceChildren(...values.map(([label, value]) => {
      const row = document.createElement('p'); const strong = document.createElement('b');
      strong.textContent = `${label}: `; row.append(strong, document.createTextNode(value)); return row;
    }));
    $('#dish-details-dialog').showModal();
  } catch (error) { showNotice(error.message, true); }
}

async function showOrderDetails(id) {
  try {
    const order = await api(`/orders/${id}`);
    $('#order-details-title').textContent = `Замовлення №${order.id}`;
    const values = [
      ['Клієнт', order.customerName], ['Страва', order.dishName], ['Кількість', order.quantity],
      ['Початкова сума', `${Number(order.originalTotalPrice).toFixed(2)} грн`],
      ['Знижка', `${order.discount}%`], ['До сплати', `${Number(order.totalPrice).toFixed(2)} грн`],
      ['Зекономлено', `${Number(order.savedMoney).toFixed(2)} грн`],
      ['Велике замовлення', order.isLargeOrder ? 'Так' : 'Ні'],
      ['Статус', order.isPaid ? `Оплачено · ${paymentLabel(order.paymentType)}` : 'Очікує оплати']
    ];
    $('#order-details-body').replaceChildren(...values.map(([label, value]) => {
      const row = document.createElement('p'); const strong = document.createElement('b');
      strong.textContent = `${label}: `; row.append(strong, document.createTextNode(value)); return row;
    }));
    $('#order-details-dialog').showModal();
  } catch (error) { showNotice(error.message, true); }
}

async function loadCustomers() {
  cache.customers = await api('/customers');
  $('#customers-body').replaceChildren(...cache.customers.map(customer => {
    const row = document.createElement('tr');
    row.append(cell(customer.name), cell(`${customer.age} років`), cell(customer.phone));
    const actions = document.createElement('td'); actions.className = 'row-actions';
    actions.append(
      actionButton('Змінити', 'Редагувати клієнта', () => editCustomer(customer)),
      actionButton('Видалити', 'Видалити клієнта', () => deleteCustomer(customer.id), true)
    );
    row.append(actions); return row;
  }));
  fillSelect($('#order-customer'), cache.customers, customer => customer.name);
  fillSelect($('#reservation-customer'), cache.customers, customer => customer.name);
  fillSelect($('#checkout-customer'), cache.customers, customer => customer.name);
  if (!$('#cart-checkout').hidden) $('#checkout-cart').disabled = cache.customers.length === 0 || cache.cart.items.length === 0;
}

function fillSelect(select, items, label) {
  const previous = select.value;
  select.replaceChildren(...items.map(item => {
    const option = document.createElement('option'); option.value = item.id; option.textContent = label(item); return option;
  }));
  if (items.some(item => String(item.id) === previous)) select.value = previous;
  select.disabled = items.length === 0;
}

function renderOrders(container, orders, includeActions = true) {
  const table = document.createElement('table');
  const head = document.createElement('thead'); const header = document.createElement('tr');
  const columns = ['№', 'Клієнт', 'Страва', 'Кількість', 'Сума', 'Знижка', 'Оплата'];
  if (includeActions) columns.push('Дії');
  for (const title of columns) { const th = document.createElement('th'); th.textContent = title; header.append(th); }
  head.append(header); table.append(head);
  const body = document.createElement('tbody');
  for (const order of orders) {
    const row = document.createElement('tr');
    const idCell = cell(''); const detailButton = document.createElement('button');
    detailButton.type = 'button'; detailButton.className = 'link-button'; detailButton.textContent = `#${order.id}`;
    detailButton.addEventListener('click', () => showOrderDetails(order.id)); idCell.append(detailButton);
    row.append(idCell, cell(order.customerName), cell(order.dishName), cell(order.quantity),
      cell(`${Number(order.totalPrice).toFixed(2)} грн`), cell(`${Number(order.discount).toFixed(0)}%`));
    const status = document.createElement('td');
    const badge = document.createElement('span'); badge.className = `tag${order.isPaid ? '' : ' busy'}`;
    badge.textContent = order.isPaid ? `Оплачено (${paymentLabel(order.paymentType)})` : 'Очікує оплати';
    status.append(badge); row.append(status);
    if (includeActions) {
      const actions = document.createElement('td'); actions.className = 'row-actions order-actions';
      if (!order.isPaid) {
        actions.append(
          actionButton('Знижка', 'Змінити знижку', () => setDiscount(order)),
          actionButton('Тест: готівка', 'Позначити як тестову оплату готівкою', () => payOrder(order.id, 'cash')),
          actionButton('Тест: картка', 'Позначити як тестову оплату карткою', () => payOrder(order.id, 'card-test')),
          actionButton('Скасувати', 'Скасувати замовлення', () => deleteOrder(order.id), true)
        );
      }
      row.append(actions);
    }
    body.append(row);
  }
  table.append(body);
  if (!orders.length) {
    const empty = document.createElement('p'); empty.className = 'empty-state'; empty.textContent = 'Замовлень ще немає.';
    container.replaceChildren(empty);
  } else container.replaceChildren(table);
}

function paymentLabel(type) {
  if (type === 'CashPayment') return 'готівка · тест';
  if (type === 'CardPayment') return 'картка · тест';
  return type || 'не вибрано';
}

async function loadOrders() {
  cache.orders = await api('/orders');
  renderOrders($('#view-orders .content-card'), cache.orders.slice().reverse());
}

async function loadTables() {
  cache.tables = await api('/tables');
  $('#tables-list').replaceChildren(...cache.tables.map(table => {
    const card = document.createElement('article'); card.className = 'table-card';
    const title = document.createElement('h3'); title.textContent = `Столик №${table.number}`;
    const details = document.createElement('p'); details.textContent = `${table.seats} місць${table.isFree ? '' : ` · Бронювання: ${table.reservedBy}`}`;
    const badge = document.createElement('span'); badge.className = `tag${table.isFree ? '' : ' busy'}`; badge.textContent = table.isFree ? 'Вільний' : 'Заброньований';
    const action = document.createElement('button'); action.className = `button ${table.isFree ? 'button-primary' : 'button-light'}`;
    action.type = 'button'; action.textContent = table.isFree ? 'Забронювати' : 'Скасувати бронювання';
    action.addEventListener('click', () => table.isFree ? openReservation(table) : cancelReservation(table.number));
    card.append(title, badge, details, action); return card;
  }));
}

async function loadCart() {
  cache.cart = await api('/cart');
  $('#cart-count').textContent = cache.cart.items.reduce((sum, item) => sum + item.quantity, 0);
  const container = $('#cart-content');
  if (!cache.cart.items.length) {
    const empty = document.createElement('p'); empty.className = 'empty-state'; empty.textContent = 'Кошик порожній. Додай страви з меню.';
    container.replaceChildren(empty); $('#cart-checkout').hidden = true; return;
  }

  const table = document.createElement('table');
  const head = document.createElement('thead'); const header = document.createElement('tr');
  for (const label of ['Страва', 'Ціна', 'Кількість', 'Сума', '']) { const th = document.createElement('th'); th.textContent = label; header.append(th); }
  head.append(header); table.append(head);
  const body = document.createElement('tbody');
  for (const item of cache.cart.items) {
    const row = document.createElement('tr'); row.append(cell(item.name), cell(`${Number(item.price).toFixed(2)} грн`));
    const quantityCell = document.createElement('td'); const quantity = document.createElement('input');
    quantity.type = 'number'; quantity.min = '1'; quantity.max = '50'; quantity.value = item.quantity; quantity.className = 'quantity-input';
    quantity.setAttribute('aria-label', `Кількість: ${item.name}`);
    quantity.addEventListener('change', () => setCartQuantity(item.dishId, Number(quantity.value)));
    quantityCell.append(quantity); row.append(quantityCell, cell(`${Number(item.lineTotal).toFixed(2)} грн`));
    const removeCell = document.createElement('td'); removeCell.append(actionButton('Видалити', 'Видалити з кошика', () => removeFromCart(item.dishId), true)); row.append(removeCell);
    body.append(row);
  }
  table.append(body);
  container.replaceChildren(table);
  $('#cart-total').textContent = `Разом: ${Number(cache.cart.total).toFixed(2)} грн`;
  $('#checkout-cart').disabled = cache.customers.length === 0;
  $('#cart-checkout').hidden = false;
}

async function refreshAll() {
  try { await Promise.all([loadDashboard(), loadDishes(), loadCustomers(), loadOrders(), loadTables(), loadCart()]); }
  catch (error) { showNotice(error.message, true); }
}

function navigate(view) {
  const validViews = ['dashboard', 'dishes', 'cart', 'customers', 'orders', 'tables'];
  const valid = validViews.includes(view) ? view : 'dashboard';
  document.querySelectorAll('.view').forEach(section => { section.hidden = section.id !== `view-${valid}`; });
  document.querySelectorAll('[data-view]').forEach(link => link.classList.toggle('active', link.dataset.view === valid));
  const titles = { dashboard: 'Огляд', dishes: 'Меню', cart: 'Кошик', customers: 'Клієнти', orders: 'Замовлення', tables: 'Столики' };
  $('#page-title').textContent = titles[valid];
}

async function addToCart(dishId) {
  try { await api('/cart/items', { method: 'POST', body: JSON.stringify({ dishId, quantity: 1 }) }); await loadCart(); showNotice('Страву додано до кошика.'); }
  catch (error) { showNotice(error.message, true); }
}

async function setCartQuantity(dishId, quantity) {
  try { await api(`/cart/items/${dishId}`, { method: 'PUT', body: JSON.stringify({ dishId, quantity }) }); await loadCart(); }
  catch (error) { showNotice(error.message, true); await loadCart(); }
}

async function removeFromCart(dishId) {
  try { await api(`/cart/items/${dishId}`, { method: 'DELETE' }); await loadCart(); showNotice('Страву видалено з кошика.'); }
  catch (error) { showNotice(error.message, true); }
}

async function checkoutCart() {
  if (!cache.cart.items.length) return showNotice('Спочатку додай страви до кошика.', true);
  if (!confirm(`Оформити замовлення на ${Number(cache.cart.total).toFixed(2)} грн?`)) return;
  try {
    const result = await api('/cart/checkout', { method: 'POST', body: JSON.stringify({ customerId: Number($('#checkout-customer').value) }) });
    await refreshAll(); navigate('orders'); location.hash = '#orders';
    showNotice(`Створено ${result.orders.length} замовлень на ${Number(result.total).toFixed(2)} грн. Обери тестовий спосіб оплати у списку.`);
  } catch (error) { showNotice(error.message, true); }
}

function editDish(dish) {
  const form = $('#dish-form'); form.dataset.editId = dish.id;
  form.elements.name.value = dish.name; form.elements.category.value = dish.category;
  form.elements.price.value = dish.price; form.elements.weight.value = dish.weight;
  $('#dish-dialog h2').textContent = 'Редагувати страву'; $('#dish-dialog .button-primary').textContent = 'Зберегти зміни';
  $('#dish-dialog').showModal();
}

async function deleteDish(id) {
  if (!confirm('Видалити цю страву з меню?')) return;
  try { await api(`/dishes/${id}`, { method: 'DELETE' }); showNotice('Страву видалено.'); await refreshAll(); }
  catch (error) { showNotice(error.message, true); }
}

function editCustomer(customer) {
  const form = $('#customer-form'); form.dataset.editId = customer.id;
  form.elements.name.value = customer.name; form.elements.age.value = customer.age; form.elements.phone.value = customer.phone;
  $('#customer-dialog h2').textContent = 'Редагувати клієнта'; $('#customer-dialog .button-primary').textContent = 'Зберегти зміни';
  $('#customer-dialog').showModal();
}

async function deleteCustomer(id) {
  if (!confirm('Видалити цього клієнта? Клієнтів із замовленнями або бронюваннями видалити не можна.')) return;
  try { await api(`/customers/${id}`, { method: 'DELETE' }); showNotice('Клієнта видалено.'); await refreshAll(); }
  catch (error) { showNotice(error.message, true); }
}

async function setDiscount(order) {
  const value = prompt(`Введи знижку від 0 до 100%. Зараз: ${order.discount}%`, String(order.discount));
  if (value === null) return;
  const percent = Number(value);
  if (!Number.isFinite(percent) || percent < 0 || percent > 100) return showNotice('Знижка має бути від 0 до 100%.', true);
  try { await api(`/orders/${order.id}/discount`, { method: 'POST', body: JSON.stringify({ percent }) }); showNotice('Знижку оновлено.'); await refreshAll(); }
  catch (error) { showNotice(error.message, true); }
}

async function payOrder(orderId, method) {
  if (!confirm('Це тестовий платіж. Реальні кошти не списуватимуться. Продовжити?')) return;
  try {
    const result = await api(`/orders/${orderId}/payment`, { method: 'POST', body: JSON.stringify({ method }) });
    showNotice(result.message); await refreshAll();
  } catch (error) { showNotice(error.message, true); }
}

async function deleteOrder(id) {
  if (!confirm('Скасувати це неоплачене замовлення?')) return;
  try { await api(`/orders/${id}`, { method: 'DELETE' }); showNotice('Замовлення скасовано.'); await refreshAll(); }
  catch (error) { showNotice(error.message, true); }
}

function openReservation(table) {
  const form = $('#reservation-form'); form.elements.tableNumber.value = table.number;
  form.elements.guests.max = table.seats; form.elements.guests.value = 1;
  $('#reservation-dialog').showModal();
}

async function cancelReservation(number) {
  try { await api(`/tables/${number}/reservation`, { method: 'DELETE' }); showNotice('Бронювання скасовано.'); await refreshAll(); }
  catch (error) { showNotice(error.message, true); }
}

document.addEventListener('click', event => {
  const nav = event.target.closest('[data-view]');
  if (nav) {
    event.preventDefault();
    const targetHash = `#${nav.dataset.view}`;
    if (location.hash !== targetHash) location.hash = targetHash;
    navigate(nav.dataset.view);
  }
  const opener = event.target.closest('[data-open]');
  if (opener) {
    const form = opener.dataset.open === 'dish-dialog' ? $('#dish-form') : opener.dataset.open === 'customer-dialog' ? $('#customer-form') : $('#order-form');
    form.reset(); delete form.dataset.editId;
    if (form.id === 'dish-form') { $('#dish-dialog h2').textContent = 'Нова страва'; $('#dish-dialog .button-primary').textContent = 'Зберегти страву'; }
    if (form.id === 'customer-form') { $('#customer-dialog h2').textContent = 'Новий клієнт'; $('#customer-dialog .button-primary').textContent = 'Додати клієнта'; }
    $(`#${opener.dataset.open}`).showModal();
  }
  if (event.target.matches('[data-close]')) event.target.closest('dialog').close();
});

window.addEventListener('hashchange', () => navigate(location.hash.slice(1)));
$('#refresh-button').addEventListener('click', refreshAll);
$('#clear-cart').addEventListener('click', async () => {
  if (!cache.cart.items.length || !confirm('Очистити кошик?')) return;
  try { await api('/cart', { method: 'DELETE' }); await loadCart(); showNotice('Кошик очищено.'); }
  catch (error) { showNotice(error.message, true); }
});
$('#checkout-cart').addEventListener('click', checkoutCart);

$('#dish-search').addEventListener('input', () => {
  clearTimeout(dishSearchTimer); dishSearchTimer = setTimeout(() => loadDishes().catch(error => showNotice(error.message, true)), 250);
});
$('#dish-category').addEventListener('change', () => loadDishes().catch(error => showNotice(error.message, true)));
$('#dish-sort').addEventListener('change', () => loadDishes().catch(error => showNotice(error.message, true)));

$('#dish-form').addEventListener('submit', async event => {
  event.preventDefault(); const form = event.currentTarget; const editing = Boolean(form.dataset.editId);
  const body = { name: form.elements.name.value.trim(), category: form.elements.category.value.trim(), price: Number(form.elements.price.value), weight: Number(form.elements.weight.value) };
  try {
    if (editing) await api(`/dishes/${form.dataset.editId}`, { method: 'PUT', body: JSON.stringify(body) });
    else await api('/dishes', { method: 'POST', body: JSON.stringify(body) });
    $('#dish-dialog').close(); delete form.dataset.editId; showNotice(editing ? 'Зміни збережено.' : 'Страву додано.'); await refreshAll();
  } catch (error) { showNotice(error.message, true); }
});

$('#customer-form').addEventListener('submit', async event => {
  event.preventDefault(); const form = event.currentTarget; const editing = Boolean(form.dataset.editId);
  const body = { name: form.elements.name.value.trim(), age: Number(form.elements.age.value), phone: form.elements.phone.value.trim() };
  try {
    if (editing) await api(`/customers/${form.dataset.editId}`, { method: 'PUT', body: JSON.stringify(body) });
    else await api('/customers', { method: 'POST', body: JSON.stringify(body) });
    $('#customer-dialog').close(); delete form.dataset.editId; showNotice(editing ? 'Дані клієнта оновлено.' : 'Клієнта додано.'); await refreshAll();
  } catch (error) { showNotice(error.message, true); }
});

$('#order-form').addEventListener('submit', async event => {
  event.preventDefault(); const form = event.currentTarget;
  try {
    await api('/orders', { method: 'POST', body: JSON.stringify({ customerId: Number(form.elements.customerId.value), dishId: Number(form.elements.dishId.value), quantity: Number(form.elements.quantity.value) }) });
    $('#order-dialog').close(); showNotice('Замовлення створено.'); await refreshAll();
  } catch (error) { showNotice(error.message, true); }
});

$('#reservation-form').addEventListener('submit', async event => {
  event.preventDefault(); const form = event.currentTarget; const number = Number(form.elements.tableNumber.value);
  try {
    await api(`/tables/${number}/reservation`, { method: 'POST', body: JSON.stringify({ customerId: Number(form.elements.customerId.value), guests: Number(form.elements.guests.value) }) });
    $('#reservation-dialog').close(); showNotice('Столик заброньовано.'); await refreshAll();
  } catch (error) { showNotice(error.message, true); }
});

navigate(location.hash.slice(1));
refreshAll();
