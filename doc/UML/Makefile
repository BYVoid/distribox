all: Network.png Overall.png

Overall.png: Overall.dot
	dot -Tpng Overall.dot -o $@

Network.png: Network.dot
	dot -Tpng Network.dot -o $@

clean:
	rm -f *.png
