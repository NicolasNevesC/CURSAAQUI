// Evita repetir try/catch em cada controller: encaminha erros para o
// errorHandler central em vez de derrubar o processo.
function asyncHandler(fn) {
  return function wrapped(req, res, next) {
    Promise.resolve(fn(req, res, next)).catch(next);
  };
}

module.exports = asyncHandler;
