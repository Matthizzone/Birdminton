# Birdminton
A bird themed badminton game.

### Shuttle Physics (```shuttle.cs```)



X Position given t

&nbsp;&nbsp;&nbsp;&nbsp; <img src="https://latex.codecogs.com/svg.latex?\Large&space;\color{white}r_x(t)=x_0+\frac{v_{0_x}}{b}(1-e^{-bt})" title="X position given t" />

X velocity given t

&nbsp;&nbsp;&nbsp;&nbsp; <img src="https://latex.codecogs.com/svg.latex?\Large&space;\color{white}v_x(t)=v_0e^{-bt}" title="X velocity given t" />

t given X position

&nbsp;&nbsp;&nbsp;&nbsp; <img src="https://latex.codecogs.com/svg.latex?\Large&space;\color{white}r_x^{-1}(t)=-\frac{1}{b}ln(1-\frac{b}{v_{0_x}}(t-x_0))" title="t given X position" />

Y position given t (Used euler's method to find zero)

&nbsp;&nbsp;&nbsp;&nbsp; <img src="https://latex.codecogs.com/svg.latex?\Large&space;\color{white}r_y(t)=h_0-\frac{g}{b}t+(\frac{g}{b^2}+\frac{v_{0_y}}{b})(1-e^{-bt})" title="Y position given t" />

Y velocity given t

&nbsp;&nbsp;&nbsp;&nbsp; <img src="https://latex.codecogs.com/svg.latex?\Large&space;\color{white}v_y(t)=-\frac{g}{b}(1-e^{-bt})+v_{0_y}e^{-bt}" title="Y velocity given t" />

Velocity vector given t

&nbsp;&nbsp;&nbsp;&nbsp; <img src="https://latex.codecogs.com/svg.latex?\Large&space;\color{white}r(t)=(r_x(t),r_y(t))" title="Velocity vector given t" />

t of flight apex

&nbsp;&nbsp;&nbsp;&nbsp; <img src="https://latex.codecogs.com/svg.latex?\Large&space;\color{white}t_{max}=-\frac{1}{b}ln(\frac{g}{v_{0_y}+g})" title="t of flight apex" />

X position max

&nbsp;&nbsp;&nbsp;&nbsp; <img src="https://latex.codecogs.com/svg.latex?\Large&space;\color{white}x_{max}=\frac{v_{0,x}}{b}" title="Velocity vector given t" />





Credit: Giovani Leone, you rock dude!
