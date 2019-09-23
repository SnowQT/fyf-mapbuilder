function fib(n)
    local function inner(m, a, b)
      if m == 0 then
        return a
      end
      return inner(m-1, b, a+b)
    end
    return inner(n, 0, 1)
  end

Citizen.CreateThread(function()
    local n = 10;
    while true do
        ProfilerEnterScope("get_fib")

        local f = fib(n);
        n = n + 10000;

        ProfilerExitScope();

        Citizen.Wait(1000)
    end
end)