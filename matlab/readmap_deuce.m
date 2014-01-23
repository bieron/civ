clear all; close all; clc;
% na podstawie obrazka o nazwie fname oblicza i zapisuje mapy: 
% woda/lad (land), 
% popytu (des), 
% obronnosci (def)
% dlugo sie wykonuje
%%
fname = 'big.jpg';
fname = 'small.jpg';
img = imread(fname);
%img = imresize( img, .3);

r = img(:,:,1);
g = img(:,:,2);
b = img(:,:,3);
%{
subplot(1,3,1); imshow(r); title('r');
subplot(1,3,2); imshow(g); title('g');
subplot(1,3,3); imshow(b); title('b');
%}
%%
desert = r>210 & g>210 & b<200;
land = (b<60 & b>35 & r+g<130) | (b>10 & b<31 & g>30 & g<80);
%land = imdilate(land, strel('disk',1));
land = imerode(land, strel('disk',1));%zgaszenie samotnych pixeli
highlands = r-g<30 & b+20<r & b<160 & b>60;
%%
foliage = land * 0;
high = foliage;
[X Y] = size(land);

for x=1:X
    for y=1:Y
        %counting adjacent highlands, base of defensibility
        c = 0;
        for i=x-1:x+1
            if i<1 || i>X, continue, end
            for j=y-1:y+1
                if j<1 || j>Y, continue, end
                c = c + highlands(i,j);
            end
        end
        high(x,y) = c;
        
        %finding plains, forests, etc
        if g(x,y)-5 > r(x,y) && g(x,y) > b(x,y)
            foliage(x,y) = g(x,y)-b(x,y);
        end
    end
end
%%
def = (high - desert - land + 1)*10;%obronnosc
waterProx = r * 0;      %bliskosc do wody
foliageProx = waterProx;%bliskosc do roslinnosci

d = 10;
for x=1+d:X-d
    for y=1+d:Y-d
        if(land(x,y) == 1)
            continue;
        end
        wsum = 0;
        gsum = 0;
        for i=x-d:x+d
            for j=y-d:y+d
                wsum = wsum + land(i,j);% 0 or 1
                gsum = gsum + (foliage(i,j)>0);
            end
        end
        waterProx(x,y) = wsum ;
        foliageProx(x,y) = gsum * 2;
        if waterProx(x,y) > 180% && waterProx(x,y) < 200
            def(x,y) = def(x,y) + waterProx(x,y)/4;
        end
   end
end

imshow(waterProx);
%%
des = uint8(~desert-land)*25;
des = des + waterProx + max(foliageProx, uint8(foliage));%popyt
%% exporting to bmp
imwrite(land, 'land.bmp');
imwrite(def, 'def.bmp');
imwrite(des, 'des.bmp');

%% saving for matlab edition
%save('vars.mat', 'des', 'def','land');
%% loading and viewing
load vars;
subplot(1,3,1); imshow(land); title('land');
subplot(1,3,2); imshow(des,[]); title('desirability');
subplot(1,3,3); imshow(def,[]); title('defensibility');
%{
subplot(2,3,1); imshow(img);
subplot(2,3,2); imshow(land);
subplot(2,3,3); imshow(foliageProx);   
subplot(2,3,4); imshow(waterProx);
subplot(2,3,5); imshow(des );
subplot(2,3,6); imshow(foliage,[] );
%}